using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Himu.Common.Service
{
    public static class UserJwtManagerExtensions
    {
        public static IServiceCollection
            AddUserJwtManager(this IServiceCollection services, JwtOptions options)
        {
            services.AddOptions();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jb =>
                    {
                        byte[] keyBytes = Encoding.UTF8.GetBytes(options.SecretToken);
                        var secretKey = new SymmetricSecurityKey(keyBytes);
                        jb.TokenValidationParameters = new()
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = secretKey
                        };
                    });
            services.AddAuthorization();
            services.TryAddScoped<IUserJwtManager, UserJwtManager>();
            return services;
        }
    }
}