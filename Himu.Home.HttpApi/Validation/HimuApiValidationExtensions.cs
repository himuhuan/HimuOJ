using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace Himu.Home.HttpApi.Validation
{
    public static class HimuApiValidationExtensions
    {
        public static IServiceCollection AddHimuApiValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}