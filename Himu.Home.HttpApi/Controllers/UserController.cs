using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core;
using Himu.HttpApi.Utility.Response;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Himu.Common.Service;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HimuMySqlContext _context;
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IMailSenderService _mailSender;
        private readonly IUserJwtManager _userJwtManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<HimuHomeUser> userManager,
            IWebHostEnvironment environment,
            HimuMySqlContext context,
            IUserJwtManager userJwtManager,
            IMailSenderService mailSender,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
            _userJwtManager = userJwtManager;
            _mailSender = mailSender;
            _logger = logger;
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<HimuUserDetailResponse>> Detail(long id)
        {
            HimuUserDetailResponse response = new();
            HimuHomeUser? user = await _context.Users
                                               .AsNoTracking()
                                               .Where(u => u.Id == id)
                                               .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                response.Failed("Bad Request in GET api/user");
                return BadRequest(response);
            }

            var userExtraData = await _context.CalculateUserSuccessRate(id)
                .AsNoTracking()
                .ToListAsync();


            var userPermission =
                HimuHomeRole.GetHighestRole(await _userManager.GetRolesAsync(user));

            #region Set User Detail Response

            response.Value.Id = user.Id;
            response.Value.UserName = user.UserName;
            response.Value.Email = user.Email;
            response.Value.PhoneNumber = user.PhoneNumber;
            response.Value.AvatarUri = user.Avatar;
            response.Value.BackgroundUri = user.Background;
            response.Value.RegisterDate = user.RegisterDate;
            response.Value.LastLoginDate = user.LastLoginDate;
            response.Value.TotalCommits = userExtraData[0].TotalSubmits;
            response.Value.ProblemSolved = userExtraData[0].ProblemSolved;
            response.Value.CommitAccepted = userExtraData[0].SuccessfulSubmits;
            response.Value.Permission = userPermission;

            #endregion

            return Ok(response);
        }

        [HttpGet("{id}/brief")]
        public async Task<ActionResult<HimuUserBriefResponse>> GetUserBrief(string id)
        {
            HimuHomeUser? user = await _userManager.FindByIdAsync(id);
            HimuUserBriefResponse response = new();
            if (user == null)
            {
                response.Failed("Bad Request in GET api/user");
                return BadRequest(response);
            }

            response.GetValue(user);
            return Ok(response);
        }

        [Route("avatar")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse<string>>> UploadUserAvatar(IFormFile avatar)
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user =
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadUserAvatar: bad request or user not exists");
                return BadRequest(response);
            }

            var savedPath = Path.Combine("images", user.Id.ToString(), "customization");
            var savedName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            UploadActionFactory uploadAction = new();
            var result = await uploadAction.MaxFileSize(3_000_000)
                                           .CheckExtensions(DefaultExtensionsChecker
                                               .ImageExtensionChecker)
                                           .SetRootPath(_environment.WebRootPath)
                                           .SaveAs(savedPath, savedName)
                                           .SaveAsync(avatar);
            if (!result.IsSuccess())
                return BadRequest(result);
            var standardPath = uploadAction.RelativePathWithFullName.Replace("\\", "/");
            user.Avatar = standardPath;
            response.Value = user.Avatar;
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

        [Route("background")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<HimuApiResponse<string>>> UploadBackround(
            IFormFile background
        )
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user =
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            var savedPath = Path.Combine("images", user.Id.ToString(), "customization");
            UploadActionFactory uploadAction = new();
            var savedName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var result = await uploadAction.MaxFileSize(5_000_000)
                                           .CheckExtensions(DefaultExtensionsChecker
                                               .ImageExtensionChecker)
                                           .SetRootPath(_environment.WebRootPath)
                                           .SaveAs(savedPath, savedName)
                                           .SaveAsync(background);
            if (!result.IsSuccess())
                return BadRequest(result);
            var standardPath = uploadAction.RelativePathWithFullName.Replace("\\", "/");
            user.Background = standardPath;
            response.Value = standardPath;
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

        [Route("avatar")]
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<HimuApiResponse>> ResetAvatar()
        {
            HimuApiResponse response = new();
            HimuHomeUser? user =
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            user.Avatar = HimuUserAssetFactory.CreateDefaultAvatar(user);
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

        [Route("background")]
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<HimuApiResponse>> ResetBackground()
        {
            HimuApiResponse response = new();
            HimuHomeUser? user =
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                response.Failed("Bad Request in UploadBackground: bad request or user not exists");
                return BadRequest(response);
            }

            user.Background = string.Empty;
            await _userManager.UpdateAsync(user);
            return Ok(response);
        }

        [HttpGet("{userId:long}/avatar")]
        public async Task<ActionResult<HimuApiResponse<string>>> GetAvatar(long userId)
        {
            HimuHomeUser? user = await _userManager.FindByIdAsync(userId.ToString());
            HimuApiResponse<string> response = new();
            if (user == null)
            {
                return NotFound(response.Failed("Resources not found"));
            }

            response.Value = user.Avatar;
            return Ok(response);
        }

        [Route("{userId:long}/background")]
        [HttpGet]
        public async Task<ActionResult<HimuApiResponse<string>>> GetBackground(long userId)
        {
            HimuHomeUser? user = await _userManager.FindByIdAsync(userId.ToString());
            HimuApiResponse<string> response = new();
            if (user == null)
            {
                return NotFound(response.Failed("Resources not found"));
            }

            response.Value = HimuUserAssetFactory.CreateDefaultBackground(user);
            return response;
        }

        [HttpPost]
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
                "User {userName} : {friendId} has been created, and the email has been sent",
                user.UserName, user.Id);
            return Ok(response);
        }

        [HttpPost("confirmation")]
        public async Task<ActionResult<HimuApiResponse>> ConfirmEmail(
            VerifyEmailConfirmationRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.EmailConfirmed)
            {
                response.Failed($"{request.UserName} 已被激活");
                return BadRequest(response);
            }
            var result = await _userManager.ConfirmEmailAsync(user, request.ConfirmationToken);
            if (!result.Succeeded)
            {
                response.Failed(result.Errors.First().Description);
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("confirmation/retry")]
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

        [HttpPost("authentication")]
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
                "User {userName}:{friendId} logged in at {time}, token: {token}",
                user.UserName, user.Id, user.LastLoginDate, response.Value.AccessToken);

            return Ok(response);
        }

        [HttpDelete("authentication")]
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

        [HttpPost("authentication/request_reset")]
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
            _mailSender.Send(user.Email, "Himu Offical: Reset Your Password",
                MailResetPasswordCodeTemplate.GetTemplate(user.UserName, token), useHtml: true);
            return Ok(response);
        }

        [HttpPut("authentication/reset")]
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

        /// <summary>
        /// Get the permissions of a user from roles.
        /// A user may have multiple roles, and each role has different permissions.
        /// But we only return the highest permission of the user.
        /// </summary>        
        [HttpGet("{userId}/authorization")]
        public async Task<ActionResult<HimuApiResponse<string>>> GetUserPermission(long userId)
        {
            HimuApiResponse<string> response = new();
            HimuHomeUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                response.Failed("BadRequest at GetUserPermission");
                return BadRequest(response);
            }

            response.Value = HimuHomeRole.GetHighestRole(await _userManager.GetRolesAsync(user));
            return Ok(response);
        }

        [HttpGet("{userId}/authorized_contests")]
        public async Task<ActionResult<AuthorizedContestsListResponse>>
            GetUserAuthorizedContests(long userId)
        {
            AuthorizedContestsListResponse response = new();
            List<AuthorizedContestsInfo> contestsUserOwned = await _context.Contests
                .AsNoTracking()
                .Where(c => c.DistributorId == userId)
                .Select(c => new AuthorizedContestsInfo(c.Id, c.Information.Title, c.Information.Code))
                .ToListAsync();
            List<AuthorizedContestsInfo> authorizedContests = await _context.ContestCreators
                .AsNoTracking()
                .Where(cc => cc.CreatorId == userId)
                .Include(cc => cc.Contest)
                .Select(cc => new AuthorizedContestsInfo(
                    cc.ContestId, cc.Contest.Information.Title, cc.Contest.Information.Code))
                .ToListAsync();
            authorizedContests.AddRange(contestsUserOwned);

            response.Success(authorizedContests);
            return Ok(response);
        }

        // TODO: add authorization
        [HttpGet("{userId}/friend_sessions")]
        public async Task<ActionResult<ChatSessionBriefResponse>> GetUserFriendSession(long userId)
        {
            ChatSessionBriefResponse response = new();
            List<ChatSessionBriefValue> chatSessions = await _context.UserChatSessions
                .AsNoTracking()
                .Where(ucs => ucs.UserId == userId || ucs.FriendId == userId)
                .Select(ucs => new ChatSessionBriefValue
                {
                    SessionId = ucs.Id,
                    Name = (userId == ucs.UserId) ? ucs.Friend.UserName : ucs.User.UserName,
                    SessionAvatarUrl = (userId == ucs.UserId) ? ucs.Friend.Avatar : ucs.User.Avatar
                })
                .ToListAsync();

            foreach (var session in chatSessions)
            {
                string? lastMessage = await _context.ChatMessages
                    .Where(cm => cm.SessionId == session.SessionId)
                    .OrderByDescending(cm => cm.SendTime)
                    .Select(cm => cm.Value)
                    .FirstOrDefaultAsync();
                if (lastMessage == null) session.LastMessage = "你们现在已经成为好友，可以发送消息了。";
                else session.LastMessage = lastMessage;
            }

            response.Success(chatSessions);
            return Ok(response);
        }

        [HttpPost("friend/{friendId}")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> RequestFriend(long friendId)
        {
            HimuApiResponse response = new();
            HimuHomeUser? userToBeFriend = await _context.Users
                .Where(u => u.Id == friendId)
                .SingleOrDefaultAsync();
            if (userToBeFriend == null)
            {
                response.Failed("No such user", HimuApiResponseCode.ResourceNotExist);
                return BadRequest(response);
            }

            long thisUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (thisUserId == friendId)
            {
                response.Failed("Cannot add yourself as friend");
                return BadRequest(response);
            }

            bool alreadyFriend = await _context.UserFriends
                .Where(uf => uf.UserId == thisUserId && uf.FriendId == friendId)
                .AnyAsync();
            if (alreadyFriend)
            {
                response.Failed("Already friend");
                return BadRequest(response);
            }

            try
            {
                await _context.UserFriends.AddAsync(new UserFriend
                {
                    UserId = thisUserId,
                    FriendId = friendId
                });

                // Each friend has a chat session
                await _context.UserChatSessions.AddAsync(new UserChatSession
                {
                    UserId = thisUserId,
                    FriendId = friendId,
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in POST api/users/{friendId}/friend", friendId);
                response.Failed("Unexpected error(s) in POST api/users/{friendId}/friend");
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}