using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility.Request;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity.Components;
using Microsoft.AspNetCore.Authorization;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly RoleManager<HimuHomeRole> _roleManager;
        private readonly HimuMySqlContext _context;

        public AuthorizationController(
            UserManager<HimuHomeUser> userManager,
            RoleManager<HimuHomeRole> roleManager,
            HimuMySqlContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        /// Different from <see> AddUserRole </see>, 
        /// this method is used for users to request higher permissions
        /// </summary>
        /// <remarks>
        ///  TODO: At present, we have not yet built a management backend.
        ///  All requests have been approved directly, but relevant records are still retained
        /// </remarks>
        [HttpPost("user/authorization")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> RequestToAddRole(
            AddUserRoleRequest request
        )
        {
            HimuApiResponse response = new();
            HimuHomeUser? user = await _userManager.FindByIdAsync(request.UserId.ToString());
            HimuHomeRole? role = await _roleManager.FindByNameAsync(request.Role);
            if (user == null || role == null)
            {
                response.Failed("BadRequest at AddUserRole");
                return BadRequest(response);
            }

            PermissionRecord record = new()
            {
                User = user,
                Role = role,
                Operation = PermissionOperation.Add,
                Date = DateTime.Now
            };
            await _context.PermissionRecords.AddAsync(record);
            await _context.SaveChangesAsync();

            // TODO: Send a message to the administrator, instead of approving directly
            if (!await _userManager.IsInRoleAsync(user, role.Name))
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get the permissions of a user from roles.
        /// A user may have multiple roles, and each role has different permissions.
        /// But we only return the highest permission of the user.
        /// </summary>        
        [HttpGet("user/{userId}/authorization")]
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
        
        
    }
}