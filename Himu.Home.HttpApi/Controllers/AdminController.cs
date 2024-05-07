using Himu.Common.Service;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Himu.HttpApi.Utility.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly RoleManager<HimuHomeRole> _roleManager;
        private readonly IUserJwtManager _userJwtManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<HimuHomeUser> userManager,
            RoleManager<HimuHomeRole> roleManager,
            IUserJwtManager userJwtManager,
            ILogger<AdminController> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userJwtManager = userJwtManager;
            _logger = logger;
        }

        [Authorize(Roles = HimuHomeRole.Administrator)]
        [HttpDelete("authentication/all")]
        public async Task<ActionResult<HimuApiResponse>> LogoutAllUser()
        {
            HimuApiResponse response = new();
            var thisUser =
                await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (thisUser == null)
            {
                response.Failed("BadRequest at LogoutAllUser");
                return BadRequest(response);
            }

            var otherUsers = _userManager.Users.Where(u => u.Id != thisUser.Id);
            await foreach (var user in otherUsers.AsAsyncEnumerable())
            {
                await _userJwtManager.InvalidateTokenAsync(user);
            }

            await _userJwtManager.InvalidateTokenAsync(thisUser);

            _logger.LogInformation(
                "The Administrator {adminId} has forced all users to go offline.", thisUser.Id);

            return Ok(response);
        }

        [HttpPost("authorization")]
        [Authorize(Roles = HimuHomeRole.Administrator)]
        public async Task<ActionResult<HimuApiResponse>> CreateNewRole(CreateNewRoleRequest request)
        {
            HimuApiResponse response = new();
            if (await _roleManager.FindByNameAsync(request.RoleName) != null)
            {
                response.Failed($"Role {request.RoleName} already exists!");
                return BadRequest(response);
            }

            await _roleManager.CreateAsync(new HimuHomeRole
            {
                Name = request.RoleName,
                NormalizedName = request.RoleName.ToUpperInvariant(),
            });
            return Ok(response);
        }
    }
}