using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Himu.HttpApi.Utility
{
    /// <summary>
    /// Filter for checking the legality of the HTTP API Action parameter.
    /// Only the action with the <see cref="HimuActionCheckAttribute"/> attribute will be checked.
    /// </summary>
    /// <remarks>
    /// The caller must ensure that the Action parameter matches the corresponding parameter 
    /// required by <see cref="HimuActionCheckAttribute"/>
    /// <para>
    /// Anyway, we will still enable additional checks when defining the <c>HIMU_DEV_MODE</c> macro. 
    /// In this case, if the Action parameter is illegal, a <see cref="HimuActionCheckFilterException"/> 
    /// exception will be thrown
    /// </para>
    /// </remarks>
    public class HimuActionCheckFilter : IAsyncActionFilter
    {
        private readonly HimuMySqlContext _contest;
        private readonly ILogger<HimuActionCheckFilter> _logger;

        public HimuActionCheckFilter(HimuMySqlContext contest,
                                     ILogger<HimuActionCheckFilter> logger)
        {
            _contest = contest;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var checkTargets = GetCheckTargets(context);
            if (checkTargets != HimuActionCheckTargets.None)
            {
                _logger.LogTrace(
                    "[{FilterName}]: Checking the parameter for Action {ActionName} (with targets={checkTargets}).",
                    nameof(HimuActionCheckFilter), context.ActionDescriptor.DisplayName, checkTargets.ToString());
                HttpChecker checker = GetChecker(checkTargets)
                    ?? throw new HimuActionCheckFilterException("The checker is not implemented.");
                CheckResult result = await checker(context);
                if (!result.CheckPassed)
                {
                    HimuApiResponse response = new();
                    response.Failed(result.Message, result.Code);
                    context.Result = new BadRequestObjectResult(response);
                    return;
                }
            }
            await next();
        }

        #region Check Methods Implementation
        private delegate Task<CheckResult> HttpChecker(ActionExecutingContext context);
        private readonly record struct CheckResult(bool CheckPassed, string Message, HimuApiResponseCode Code);

        /// <summary>
        /// Check the action has the <see cref="HimuActionCheckAttribute"/> attribute.
        /// </summary>
        private static HimuActionCheckTargets GetCheckTargets(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                if (descriptor.MethodInfo.GetCustomAttributes(typeof(HimuActionCheckAttribute), false)
                    .FirstOrDefault() is HimuActionCheckAttribute target)
                {
                    return target.Target;
                }
            }
            return HimuActionCheckTargets.None;
        }

        private HttpChecker? GetChecker(HimuActionCheckTargets target)
        {
            return target switch
            {
                HimuActionCheckTargets.ContestDistributor => CheckUserIsContestDistributor,
                HimuActionCheckTargets.ProblemModify => CheckProblemModify,
                _ => null
            };
        }

        private async Task<CheckResult> CheckUserIsContestDistributor(ActionExecutingContext context)
        {
#if HIMU_DEV_MODE
            if (!context.ActionArguments.ContainsKey("contestId"))
            {
                throw new HimuActionCheckFilterException("The parameter 'contestId' is required.");
            }
#endif
            long userId = long.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            long contestId = (long) context.ActionArguments["contestId"]!;
            bool isAdministrator = context.HttpContext.User.IsInRole("Administrator");
            HimuContest? contest = await _contest.Contests
                .Where(c => c.Id == contestId)
                .SingleOrDefaultAsync();
            if (contest == null || (contest.DistributorId != userId && !isAdministrator))
            {
                return new CheckResult(false, $"Contest {contestId} not found or you don't have permission.",
                                       HimuApiResponseCode.BadAuthentication);
            }
            return new CheckResult(true, string.Empty, HimuApiResponseCode.Succeed);
        }

        private async Task<CheckResult> CheckProblemModify(ActionExecutingContext context)
        {
#if HIMU_DEV_MODE
            if (!context.ActionArguments.ContainsKey("problemId"))
            {
                throw new HimuActionCheckFilterException("The parameter 'problemId' is required.");
            }
#endif
            long userId = long.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isAdministrator = context.HttpContext.User.IsInRole("Administrator");
            long problemId = (long) context.ActionArguments["problemId"]!;

            var permissionInfo = await _contest.ProblemSet
                .AsNoTracking()
                .Where(p => p.Id == problemId)
                .Select(p => new
                {
                    CreatorId = p.DistributorId,
                    ContestDistributorId = p.Contest.DistributorId,
                })
                .SingleOrDefaultAsync();
            if (permissionInfo == null
                || (permissionInfo.CreatorId != userId && permissionInfo.ContestDistributorId != userId && !isAdministrator))
            {
                return new CheckResult(false, $"Problem {problemId} not found or you don't have permission.",
                                       HimuApiResponseCode.BadAuthentication);
            }
            return new CheckResult(true, string.Empty, HimuApiResponseCode.Succeed);
        }
        #endregion
    }
}
