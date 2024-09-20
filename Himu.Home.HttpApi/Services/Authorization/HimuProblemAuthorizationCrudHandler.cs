using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Services.Authorization
{
    /// <summary>
    /// Authorization handler for HimuProblem Curd operations.
    /// </summary>
    /// <remarks>
    /// Do not register this handler as singleton, because it has a dependency on the database context.
    /// see also <c> https://learn.microsoft.com/en-us/aspnet/core/security/authorization/dependencyinjection </c>
    /// </remarks>
    public class HimuProblemAuthorizationCrudHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, HimuProblem>
    {
        private readonly HimuOnlineJudgeContext _context;

        public HimuProblemAuthorizationCrudHandler(HimuOnlineJudgeContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            HimuProblem resource)
        {
            // Administrator can do anything.
            // Anyone can read the problem.
            if (context.User.IsInRole(HimuHomeRole.Administrator)
                || requirement.Name == HimuCrudOperations.Read.Name)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // ofc, the distributor of the contest can do anything.
            long userId = long.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            long contestDistributorId = _context.Contests
                .Where(c => c.Id == resource.ContestId)
                .Select(c => c.DistributorId)
                .SingleOrDefaultAsync().Result;
            if (contestDistributorId == userId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            bool hasPermission = _context.ContestCreators
                .Where(cc => cc.ContestId == resource.ContestId && cc.CreatorId == userId)
                .AnyAsync().Result;
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}