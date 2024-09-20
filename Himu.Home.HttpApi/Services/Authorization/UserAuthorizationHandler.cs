using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Services.Authorization
{
    public class UserAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, HimuHomeUser>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            HimuHomeUser resource)
        {
            // only administrators can  delete users
            if (requirement.Name == HimuCrudOperations.Delete.Name 
                && context.User.IsInRole(HimuHomeRole.Administrator))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            // only administrators or user himself can update user
            var userId = long.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (requirement.Name == HimuCrudOperations.Update.Name
                && (context.User.IsInRole(HimuHomeRole.Administrator) || userId == resource.Id))
            {
                context.Succeed(requirement);
            }
            // anyone can read, create the user
            else if (requirement.Name == HimuCrudOperations.Read.Name
                || requirement.Name == HimuCrudOperations.Create.Name)
            {
                context.Succeed(requirement);
            }
            // authorization failed
            return Task.CompletedTask;
        }
    }
}
