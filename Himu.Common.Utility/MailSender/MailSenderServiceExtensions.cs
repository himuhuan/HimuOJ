using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Himu.Common.Service
{
    public static class MailSenderServiceExtensions
    {
        public static IServiceCollection
            AddMailSenderService(this IServiceCollection services, Action<MailSenderOptions> optionsBuilder)
        {
            services.AddLogging().AddOptions();
            services.PostConfigure(optionsBuilder);
            services.TryAddScoped<IMailSenderService, MailSenderService>();
            return services;
        }

        public static IServiceCollection AddMailSenderService(this IServiceCollection services)
        {
            AddMailSenderService(services, _ => { });
            return services;
        }
    }
}