using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Filters
{
    public class AccessTokenCheckFilter : IAsyncActionFilter
    {
        private readonly IDistributedCache _cache;

        public AccessTokenCheckFilter(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            bool enableCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                bool noCheck
                    = descriptor.MethodInfo
                                .GetCustomAttributes(typeof(NoAccessTokenCheckAttribute), false)
                                .Any();
                bool authorized
                    = descriptor.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                                .Any();
                if (authorized && !noCheck)
                    enableCheck = true;
            }

            if (enableCheck)
            {
                string key
                    = $"access_token_cache_{context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)}";
                string? expectedToken = await _cache.GetStringAsync(key);
                string contextToken = context.HttpContext.Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(expectedToken)
                    || string.IsNullOrEmpty(contextToken)
                    || !contextToken.StartsWith("Bearer")
                    || expectedToken != contextToken[7..])
                {
                    context.Result = new ObjectResult("illegal token")
                    {
                        StatusCode = 401
                    };
                    return;
                }
            }

            await next();
        }
    }
}