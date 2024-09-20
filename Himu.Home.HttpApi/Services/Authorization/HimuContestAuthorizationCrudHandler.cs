using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Services.Authorization
{
    public class HimuContestAuthorizationCrudHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, HimuContest>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            HimuContest resource)
        {
            // ofc, the Administrator can do anything.
            if (context.User.IsInRole(HimuHomeRole.Administrator))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (requirement.Name == HimuCrudOperations.Create.Name
                && context.User.IsInRole(HimuHomeRole.ContestDistributor))
            {
                context.Succeed(requirement);
            }
            // only the Administrator or the distributor of this contest can delete or update the contest
            else if (requirement.Name == HimuCrudOperations.Delete.Name
                    || requirement.Name == HimuCrudOperations.Update.Name)
            {
                var userId = long.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (resource.DistributorId == userId)
                    context.Succeed(requirement);
            }
            // anyone can read the contest
            else
            {
                context.Equals(requirement);
            }
            return Task.CompletedTask;
        }
    }
}