using Himu.Common.Service;
using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Authorization;
using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Storage;
using Himu.Home.HttpApi.Utils;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMailSenderService _mailSender;
        private readonly ILogger<UserController> _logger;
        private readonly IObjectStorageService _objectStorageService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IOJContextService _oJContextService;

        public UserController(
            IMailSenderService mailSender,
            ILogger<UserController> logger,
            IIdentityContextService identityContextService,
            IObjectStorageService objectStorageService,
            IOJContextService oJContextService,
            IAuthorizationService authorizationService)
        {
            _mailSender = mailSender;
            _logger = logger;
            _identityContextService = identityContextService;
            _objectStorageService = objectStorageService;
            _oJContextService = oJContextService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<HimuUserDetailResponse>> Detail(long id)
        {
            HimuUserDetailResponse response = new();
            try
            {
                response.Value = await _identityContextService.GetUserDetail(id);
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            return response.ActionResult();
        }

        [HttpGet("{id}/brief")]
        public async Task<ActionResult<HimuUserBriefResponse>> GetUserBrief(long id)
        {
            HimuUserBriefResponse response = new();
            try
            {
                response.Value = await _identityContextService.GetUserBrief(id);
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }
            return response.ActionResult();
        }

        [Route("avatar")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse<string>>> UploadUserAvatar(IFormFile avatar)
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user =
                await _identityContextService.FindUserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadUserAvatar: bad request or user not exists");
                return BadRequest(response);
            }

            try
            {
                string path = await _objectStorageService.SaveUserAvatar(user, avatar);
                user.Avatar = path;
                await _identityContextService.UpdateUser(user);
                response.Value = path;
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            return response.ActionResult();
        }

        [Route("background")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse<string>>> UploadBackground(
            IFormFile background
        )
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user =
                await _identityContextService.FindUserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            try
            {
                string path = await _objectStorageService.SaveUserBackground(user, background);
                user.Background = path;
                await _identityContextService.UpdateUser(user);
                response.Value = path;
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            return response.ActionResult();
        }

        [Route("avatar")]
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<HimuApiResponse>> ResetAvatar()
        {
            HimuApiResponse response = new();
            HimuHomeUser? user =
                await _identityContextService.FindUserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            try
            {
                string newAvatar = _objectStorageService.ResetUserAvatar(user);
                user.Avatar = newAvatar;
                await _identityContextService.UpdateUser(user);
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            return response.ActionResult();
        }

        [Route("background")]
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<HimuApiResponse>> ResetBackground()
        {
            HimuApiResponse response = new();
            HimuHomeUser? user =
                await _identityContextService.FindUserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            try
            {
                string newBackground = _objectStorageService.ResetUserBackground(user);
                user.Background = newBackground;
                await _identityContextService.UpdateUser(user);
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            return response.ActionResult();
        }

        [HttpGet("{userId:long}/avatar")]
        public async Task<ActionResult<HimuApiResponse<string>>> GetAvatar(long userId)
        {
            HimuHomeUser? user = await _identityContextService.FindUserById(userId.ToString());
            HimuApiResponse<string> response = new();
            if (user == null)
                response.Failed("no such user", HimuApiResponseCode.ResourceNotExist);
            else
                response.Value = user.Avatar;
            return response.ActionResult();
        }

        [Route("{userId:long}/background")]
        [HttpGet]
        public async Task<ActionResult<HimuApiResponse<string>>> GetBackground(long userId)
        {
            HimuHomeUser? user = await _identityContextService.FindUserById(userId.ToString());
            HimuApiResponse<string> response = new();
            if (user == null)
                response.Failed("no such user", HimuApiResponseCode.ResourceNotExist);
            else
                response.Value = user.Background;
            return response.ActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<HimuApiResponse>> Register(UserRegisterRequest request)
        {
            HimuApiResponse response = new();

            if (await _identityContextService.FindUserByName(request.Mail) != null
                || await _identityContextService.FindUserByName(request.UserName) != null)

            {
                response.Failed(
                    $"{request.UserName} or {request.Mail} has already exists!", HimuApiResponseCode.DuplicateItem);
                return BadRequest(response);
            }

            var user = _objectStorageService.CreateUser(request);
            var result = await _identityContextService.AddUser(user, request.Password);
            if (!result.Succeeded)
            {
                response.Failed($"cannot create user {user.UserName}: {result.Errors}");
                return BadRequest(response);
            }

            string verifyToken = await _identityContextService.GenerateEmailConfirmationToken(user);
            _mailSender.Send(user.Email, "Himu 客服酱",
                MailActivationCodeTemplate.GetTemplate(user.UserName, verifyToken), useHtml: true);

            _logger.LogInformation(
                "User {userName} : {userId} has been created, and the email has been sent",
                user.UserName, user.Id);
            return Ok(response);
        }

        [HttpPost("confirmation/retry")]
        public async Task<ActionResult<HimuApiResponse>> ResentEmailConfirmation(
            ResentEmailConfirmationRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _identityContextService.FindUserByName(request.UserName);
            if (user == null || user.EmailConfirmed || user.Email != request.Mail)
            {
                response.Failed($"{request.UserName} 与绑定的邮箱不符, 或已被激活");
                return BadRequest(response);
            }

            string verifyToken = await _identityContextService.GenerateEmailConfirmationToken(user);
            _mailSender.Send(user.Email, "Himu 客服酱",
                MailResetPasswordCodeTemplate.GetTemplate(user.UserName, verifyToken),
                useHtml: true);
            return Ok(response);
        }

        [HttpPost("authentication")]
        public async Task<ActionResult<HimuLoginResponse>> Login(UserLoginRequest request)
        {
            HimuLoginResponse response = new();

            HimuHomeUser? user =
                await _identityContextService.FindUser(request.Method, request.Input);

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

            var lockedTime = await _identityContextService.UserLockedOut(user);
            if (lockedTime != null)
            {
                response.Failed($"账号已被锁定，请在 {lockedTime!.Value.Seconds} 秒重试",
                    HimuApiResponseCode.LockedUser);
                return BadRequest(response);
            }

            string? token = null;
            try
            {
                token = await _identityContextService.CheckUserPassword(user, request.Password);
            }
            catch (HimuApiException e)
            {
                response.Failed(e.Message, e.Code);
            }

            if (token != null)
            {
                _logger.LogDebug(
                    "User {userName}:{userId} logged in at {time}, token: {token}",
                    user.UserName, user.Id, user.LastLoginDate, response.Value.AccessToken);
                response.Success(user, token);
            }

            return response.ActionResult();
        }

        [HttpDelete("authentication")]
        public async Task<ActionResult<HimuApiResponse>> Logout()
        {
            var response = new HimuApiResponse();
            HimuHomeUser? user
                = await _identityContextService.FindUserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Unexpected error(s) in Logout");
                return BadRequest(response);
            }

            await _identityContextService.InvalidateToken(user);
            return Ok(response);
        }

        [HttpPost("authentication/request_reset")]
        public async Task<ActionResult<HimuApiResponse>> SendResetEmailConfirmation(string email)
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _identityContextService.FindUserByEmail(email);
            if (user == null)
            {
                response.Failed("no such user", HimuApiResponseCode.ResourceNotExist);
                return BadRequest(response);
            }

            string token = await _identityContextService.GeneratePasswordResetToken(user);
            _mailSender.Send(user.Email, "Himu Official: Reset Your Password",
                MailResetPasswordCodeTemplate.GetTemplate(user.UserName, token), useHtml: true);
            return Ok(response);
        }

        [HttpPut("authentication/reset")]
        public async Task<ActionResult<HimuApiResponse>> ResetUserPassword(
            ResetUserPasswordRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _identityContextService.FindUserByEmail(request.Mail);
            if (user == null)
            {
                response.Failed("Unexpected error(s) in POST authentication/reset");
                return BadRequest(response);
            }

            var result =
                await _identityContextService.ResetPassword(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                response.Failed(result.Errors.First().Description);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get the permissions of a user from roles.
        /// A user may have multiple roles, and each role has different permissions.
        /// But we only return the highest permission of the user.
        /// </summary>
        [HttpGet("{userId}/authorization")]
        public async Task<ActionResult<HimuApiResponse<string>>> GetUserPermission(long userId)
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user = await _identityContextService.FindUserById(userId.ToString());
            if (user == null)
            {
                response.Failed("BadRequest at GetUserPermission");
                return BadRequest(response);
            }
            response.Value = await _identityContextService.GetUserHighestRole(user);
            return Ok(response);
        }

        [HttpGet("{userId}/authorized_contests")]
        public async Task<ActionResult<AuthorizedContestsListResponse>> GetUserAuthorizedContests(long userId)
        {
            AuthorizedContestsListResponse response = new();
            var authorizedContests = await _oJContextService.GetUserAuthorizedContests(userId);
            response.Success(authorizedContests);
            return Ok(response);
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> UpdateUser(long userId, HimuUserDetail newDetail)
        {
            HimuApiResponse response = new();

            HimuHomeUser? user = await _identityContextService.FindUserById(userId);
            if (user == null)
            {
                response.Failed("No such user", HimuApiResponseCode.ResourceNotExist);
                return NotFound(response);
            }

            if (userId != newDetail.Id
                || !(await _authorizationService.AuthorizeAsync(User, user, HimuCrudOperations.Update)).Succeeded)
            {
                response.Failed("Authorization failed", HimuApiResponseCode.BadAuthorization);
                return BadRequest(response);
            }

            await _identityContextService.UpdateUser(user, newDetail);
            return Ok(response);
        }
    }
}