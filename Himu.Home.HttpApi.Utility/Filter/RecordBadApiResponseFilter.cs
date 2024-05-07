using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Himu.HttpApi.Utility.Utility
{
    public class RecordBadApiResponseFilter : IAsyncResultFilter
    {
        private readonly ILogger<RecordBadApiResponseFilter> _logger;
        private readonly IDistributedCache _redisCache;

        public RecordBadApiResponseFilter(ILogger<RecordBadApiResponseFilter> logger, IDistributedCache redisCache)
        {
            _logger = logger;
            _redisCache = redisCache;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult result && result.StatusCode >= 300)
            {
                string connectionId = context.HttpContext.Connection.Id;
                string cacheKey = $"user_{connectionId}_{context.ActionDescriptor.Id}_failed_count";
                string? failedCountData = await _redisCache.GetStringAsync(cacheKey);
                int failedCount = failedCountData == null ? 0 : JsonSerializer.Deserialize<int>(failedCountData);
                if (result.Value is HimuApiResponse value)
                {
                    if (value.Code != HimuApiResponseCode.UnexpectedError)
                    {
                        _logger.LogInformation("(from {id}) {action} failed - {@code}: {@message}",
                            connectionId, context.ActionDescriptor.DisplayName,
                            value.Code, value.Message);
                    }
                    else
                    {
                        _logger.LogError("(from {id}) {action} failed - {@code}: {@message}",
                            connectionId, context.ActionDescriptor.DisplayName,
                            value.Code, value.Message);
                        // UnexpectedError often carry sensitive information related to development
                        value.Message = "Internal errors happened";
                    }
                }
                else
                {
                    _logger.LogInformation("(from {id}) {action} failed: {@result}",
                        connectionId, context.ActionDescriptor.DisplayName,
                        result.Value);
                }
                failedCount++;
                if (failedCount >= 10)
                {
                    _logger.LogWarning("{id} try {name} failed too many times",
                        connectionId, context.ActionDescriptor.DisplayName);
                }
                await _redisCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(failedCount), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });
            }
            await next();
        }
    }
}