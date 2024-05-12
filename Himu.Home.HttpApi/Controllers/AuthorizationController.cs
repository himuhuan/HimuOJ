using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility.Request;
using Himu.HttpApi.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Evaluation;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly RoleManager<HimuHomeRole> _roleManager;
        private readonly HimuMySqlContext _context;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(
            UserManager<HimuHomeUser> userManager,
            RoleManager<HimuHomeRole> roleManager,
            HimuMySqlContext context,
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Different from <see> AddUserRole </see>, 
        /// this method is used for users to request higher permissions
        /// </summary>
        /// <remarks>
        ///  TODO: At present, we have not yet built a management backend.
        ///  All requests have been approved directly, but relevant records are still retained
        /// </remarks>
        [HttpPost]
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

        [HttpPost("check/permission/crud_operation")]
        [Authorize]
        public async Task<IActionResult> CheckCrudPermission(
            [FromBody] CheckCurdPermissionRequest request
        )
        {
            var checkPermissionMethod = GetCheckPermssionMethod(request.EntityType);
            AuthorizationResult? result = await checkPermissionMethod(request.EntityId, request.Operation);
            
            if (result == null)
                return NotFound();
            if (!result.Succeeded)
                return Forbid();
            return Ok();
        }

        #region Private Methods & Members

        private Func<long, OperationAuthorizationRequirement, Task<AuthorizationResult?>>
            GetCheckPermssionMethod(string entityType)
        {
            return entityType switch
            {
                "problem" => CheckProblemPermission,
                "contest" => CheckContestPermission,
                // because the entityType is validated by the validator,
                // it impossible to reach here
                _ => (entityId, operation) => Task.FromResult<AuthorizationResult?>(null),
            };
        }

        private async Task<AuthorizationResult?> CheckContestPermission(   
            long contestId,
            OperationAuthorizationRequirement operation
        )
        {
            HimuContest? contest = await _context.Contests.AsNoTracking()
                                                          .Where(c => c.Id == contestId)
                                                          .SingleOrDefaultAsync();
            if (contest == null)
                return null;
            return await _authorizationService.AuthorizeAsync(User, contest, operation);
        }

        private async Task<AuthorizationResult?> CheckProblemPermission(
            long problemId,
            OperationAuthorizationRequirement operation
        )
        {
            HimuProblem? problem = await _context.ProblemSet.AsNoTracking()
                                                            .Where(p => p.Id == problemId)
                                                            .SingleOrDefaultAsync();
            if (problem == null)
                return null;
            return await _authorizationService.AuthorizeAsync(User, problem, operation);
        }
        #endregion
    }
}