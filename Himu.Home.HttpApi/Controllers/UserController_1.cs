using Himu.Common.Service;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMailSenderService _mailSender;
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly IUserJwtManager _userJwtManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IMailSenderService mailSender,
            UserManager<HimuHomeUser> userManager,
            IUserJwtManager userJwtManager,
            ILogger<UserController> logger
        )
        {
            _mailSender = mailSender;
            _userManager = userManager;
            _userJwtManager = userJwtManager;
            _logger = logger;
        }

        [HttpPost("users")]
        public async Task<ActionResult<HimuApiResponse>> Register(UserRegisterRequest request)
        {
            HimuApiResponse response = new();

            if (await _userManager.FindByEmailAsync(request.Mail) != null
                || await _userManager.FindByNameAsync(request.UserName) != null)

            {
                response.Failed($"{request.UserName} or {request.Mail} has already exists!");
                return BadRequest(response);
            }

            var user = HimuHomeUserFactory.CreateUserFromRequest(request);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                response.Failed($"cannot create user {user.UserName}: {result.Errors}");
                return BadRequest(response);
            }

            string verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _mailSender.Send(user.Email, "Himu 客服酱",
                MailActivationCodeTemplate.GetTemplate(user.UserName, verifyToken), useHtml: true);

            _logger.LogInformation(
                "User {userName} : {userId} has been created, and the email has been sent",
                user.UserName, user.Id);
            return Ok(response);
        }

        [Route("users/confirmation/retry")]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse>> ResentEmailConfirmation(
            ResentEmailConfirmationRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.EmailConfirmed || user.Email != request.Mail)
            {
                response.Failed($"{request.UserName} 与绑定的邮箱不符, 或已被激活");
                return BadRequest(response);
            }

            string verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _mailSender.Send(user.Email, "Himu 客服酱",
                MailResetPasswordCodeTemplate.GetTemplate(user.UserName, verifyToken),
                useHtml: true);
            return Ok(response);
        }

        [Route("users/confirmation")]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse>> VerifyEmailConfirmation(
            VerifyEmailConfirmationRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.EmailConfirmed)
            {
                response.Failed($"{request.UserName} not exists or has already confirmation!");
                return BadRequest(response);
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.ConfirmationToken);
            if (!result.Succeeded)
            {
                response.Failed($"{result.Errors.First().Description}");
                return BadRequest(response);
            }

            // user.EmailConfirmed = true;
            _mailSender.Send(user.Email, "Himu 客服酱",
                MailActivationCompletedTemplate.GetTemplate(user.UserName), useHtml: true);
            return Ok(response);
        }

        [Route("users/authentication")]
        [HttpPost]
        public async Task<ActionResult<HimuLoginResponse>> Login(UserLoginRequest request)
        {
            HimuLoginResponse response = new();

            HimuHomeUser? user =
                await _userManager.FindByMethodAsync(request.Method, request.Input);

            if (user == null)
            {
                response.Failed("错误的用户名或密码", HimuApiResponseCode.BadAuthentication);
                return BadRequest(response);
            }

            if (!user.EmailConfirmed)
            {
                response.Failed("用户没有激活", HimuApiResponseCode.BadAuthentication);
                return Unauthorized(response);
            }

            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
            {
                var retryTime =
                    await _userManager.GetLockoutEndDateAsync(user) - DateTimeOffset.UtcNow;
                response.Failed(
                    $"账号已被锁定，请在 {retryTime!.Value.Seconds} 秒重试",
                    HimuApiResponseCode.LockedUser);
                return BadRequest(response);
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                await _userManager.AccessFailedAsync(user);
                response.Failed("错误的用户名或密码", HimuApiResponseCode.BadAuthentication);
                return BadRequest(response);
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            user.LastLoginDate = DateOnly.FromDateTime(DateTime.Now);
            await _userManager.UpdateAsync(user);
            response.GetInfo(user, await _userJwtManager.GetAccessTokenAsync(user));

            _logger.LogDebug(
                "User {userName}:{userId} logged in at {time}, token: {token}",
                user.UserName, user.Id, user.LastLoginDate, response.Value.AccessToken);

            return Ok(response);
        }

        [Route("users/authentication")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> Logout()
        {
            var response = new HimuApiResponse();
            HimuHomeUser? user
                = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Unexpected error(s) in Logout");
                return BadRequest(response);
            }

            await _userJwtManager.InvalidateTokenAsync(user);
            return Ok(response.Success("The user is logged out"));
        }

        [Route("users/authentication/request_reset")]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse>> SendResetEmailConfirmation(string email)
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.Failed("Unexpected error(s) in POST authentication/reset");
                return BadRequest(response);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _mailSender.Send(user.Email, "Himu 客服酱: 重置密码",
                MailResetPasswordCodeTemplate.GetTemplate(user.UserName, token), useHtml: true);
            return Ok(response);
        }

        [Route("users/authentication/reset")]
        [HttpPut]
        public async Task<ActionResult<HimuApiResponse>> ResetUserPassword(
            ResetUserPasswordRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByEmailAsync(request.Mail);
            if (user == null)
            {
                response.Failed("Unexpected error(s) in POST authentication/reset");
                return BadRequest(response);
            }

            var result =
                await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                response.Failed(result.Errors.First().Description);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}