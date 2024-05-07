using Himu.EntityFramework.Core.Entity;
using Himu.EntityFramework.Core;
using Himu.HttpApi.Utility.Response;
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
        private readonly HimuMySqlContext _context;
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public UserController(
            UserManager<HimuHomeUser> userManager,
            IWebHostEnvironment environment,
            HimuMySqlContext context,
            AuthorizationController authorizationServices
        )
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
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

            var userExtraData = await
                _context.UserCommits
                        .AsNoTracking()
                        .Where(c => c.UserId == id)
                        .GroupBy(c => c.UserId)
                        .Select(g => new
                        {
                            CommitTotalCount = g.Count(),
                            AcceptedProblemCount = g
                                                   .Where(c =>
                                                       c.Status == ExecutionStatus.ACCEPTED)
                                                   .Select(c => c.ProblemId)
                                                   .Distinct()
                                                   .Count(),
                            AcceptedCommitCount = g.Count(c =>
                                c.Status == ExecutionStatus.ACCEPTED)
                        })
                        .SingleOrDefaultAsync();

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
            response.Value.TotalCommits = userExtraData?.CommitTotalCount ?? 0;
            response.Value.ProblemSolved = userExtraData?.AcceptedProblemCount ?? 0;
            response.Value.CommitAccepted = userExtraData?.AcceptedCommitCount ?? 0;
            response.Value.Permission = userPermission ?? HimuHomeRole.StandardUser;

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
    }
}