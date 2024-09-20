using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Request;
using Himu.Home.HttpApi.Response;
using Himu.Home.HttpApi.Services.Context;
using Himu.HttpApi.Utility.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<HimuHomeUser> _userManager;
        private readonly RoleManager<HimuHomeRole> _roleManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IOJContextService _oJContextService;
        private readonly IIdentityContextService _identityContextService;

        public AuthorizationController(
            UserManager<HimuHomeUser> userManager,
            RoleManager<HimuHomeRole> roleManager,
            IIdentityContextService identityContextService,
            IAuthorizationService authorizationService,
            IOJContextService oJContextService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _identityContextService = identityContextService;
            _authorizationService = authorizationService;
            _oJContextService = oJContextService;
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

            await _identityContextService.AddPermissionRecords(user, role);

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
            var checkPermissionMethod = GetCheckPermissionMethod(request.EntityType);
            AuthorizationResult? result = await checkPermissionMethod(request.EntityId, request.Operation);

            if (result == null)
                return NotFound();
            if (!result.Succeeded)
                return Forbid();
            return Ok();
        }

        #region Private Methods & Members

        private Func<long, OperationAuthorizationRequirement, Task<AuthorizationResult?>>
            GetCheckPermissionMethod(string entityType)
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
            HimuContest? contest = await _oJContextService.GetContest(contestId);
            if (contest == null)
                return null;
            return await _authorizationService.AuthorizeAsync(User, contest, operation);
        }

        private async Task<AuthorizationResult?> CheckProblemPermission(
            long problemId,
            OperationAuthorizationRequirement operation
        )
        {
            HimuProblem? problem = await _oJContextService.GetProblem(problemId);
            if (problem == null)
                return null;
            return await _authorizationService.AuthorizeAsync(User, problem, operation);
        }

        #endregion Private Methods & Members
    }
}