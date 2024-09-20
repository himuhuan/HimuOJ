using Himu.Home.HttpApi.Services.Context;
using Himu.Home.HttpApi.Services.Context.Implement;
using Himu.Home.HttpApi.Services.Judge;
using Himu.Home.HttpApi.Services.Judge.Implement;
using Himu.Home.HttpApi.Services.Storage;
using Himu.Home.HttpApi.Services.Storage.Implement;

namespace Himu.Home.HttpApi.Services
{
    public static class HimuServiceExtension
    {
        public static IServiceCollection AddHimuContextServices(this IServiceCollection services)
        {
            services.AddScoped<IOJContextService, OJContextService>();
            services.AddScoped<IIdentityContextService, IdentityContextService>();
            services.AddSingleton<IObjectStorageService, LocalObjectStorageService>();
            return services;
        }

        public static IServiceCollection AddHimuJudgeCoreServices(this IServiceCollection services)
        {
            services.AddGrpcClient<GrpcJudgeClient.JudgeService.JudgeServiceClient>(options =>
            {
                options.Address = new Uri("http://localhost:7721");
            });
            services.AddSingleton<IJudgeTaskDispatcher, GrpcJudgeTaskDispatcher>();
            return services;
        }
    }
}
