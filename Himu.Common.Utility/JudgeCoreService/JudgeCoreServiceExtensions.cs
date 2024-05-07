using Himu.Common.Service.JudgeCoreService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Himu.Common.Service
{
    public static class JudgeCoreServiceExtensions
    {
        public static IServiceCollection AddJudgeCoreService(this IServiceCollection services)
        {
            services.AddOptions();
            services.TryAddSingleton<IJudgeCoreService, JudgeSocketService>();
            return services;
        }
    }
}
