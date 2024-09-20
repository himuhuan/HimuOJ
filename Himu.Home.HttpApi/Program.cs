// Copyright(c) 2023 Himu (himu.liuhuan@gmail.com)

using Himu.Common.Service;
using Himu.EntityFramework.Core.Contests;
using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Filters;
using Himu.Home.HttpApi.Hubs;
using Himu.Home.HttpApi.Services;
using Himu.Home.HttpApi.Services.Authorization;
using Himu.Home.HttpApi.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System.Diagnostics;
using Yitter.IdGenerator;

Log.Logger = new LoggerConfiguration()
             .WriteTo
             .Console()
             .MinimumLevel
             .Debug()
             .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Id generator
    IdGeneratorOptions idGeneratorOptions = new(1);
    YitIdHelper.SetIdGenerator(idGeneratorOptions);

    #region Logging

    builder.Host.UseSerilog((context, services, config) =>
    {
        config.ReadFrom
              .Configuration(context.Configuration)
              .ReadFrom
              .Services(services)
              .MinimumLevel
              .Override("Microsoft.AspNetCore", LogEventLevel.Warning)
              .MinimumLevel
              .Override("Microsoft.EntityFrameworkCore", LogEventLevel.Verbose)
              .Enrich
              .FromLogContext()
              .WriteTo
              .Console();
    });

    #endregion Logging

    // Add services to the container.

    #region Filters

    builder.Services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add<RecordBadApiResponseFilter>();
    });

    #endregion Filters

    #region Validation

    builder.Services.AddHimuApiValidation();

    #endregion Validation

    #region EFCore Options

#pragma warning disable CS0618 // 类型或成员已过时
    string? connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
    Debug.Assert(connectionString != null);
    builder.Services.AddDbContext<HimuContext>(options =>
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });
    builder.Services.AddDbContext<HimuIdentityContext>(options =>
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });
    builder.Services.AddDbContext<HimuOnlineJudgeContext>(options =>
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });

    builder.Services.AddHimuContextServices();

#pragma warning restore CS0618 // 类型或成员已过时

    #endregion EFCore Options

    #region Identity Options

    builder.Services.AddDataProtection();
    builder.Services.AddIdentityCore<HimuHomeUser>(options =>
    {
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);

        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;

        // Use shorter and easier-to-remember verification code formats when sending email verification codes.
        options.Tokens.PasswordResetTokenProvider
            = TokenOptions.DefaultEmailProvider;
        options.Tokens.EmailConfirmationTokenProvider
            = TokenOptions.DefaultEmailProvider;
    });
    IdentityBuilder identityBuilder =
        new(typeof(HimuHomeUser), typeof(HimuHomeRole), builder.Services);
    identityBuilder.AddEntityFrameworkStores<HimuIdentityContext>()
                   .AddDefaultTokenProviders()
                   .AddRoleManager<RoleManager<HimuHomeRole>>()
                   .AddUserManager<UserManager<HimuHomeUser>>();
    var jwtOptionsSection = builder.Configuration.GetSection("JwtOptions");
    JwtOptions jwtOptions = new();
    jwtOptionsSection.Bind(jwtOptions);
    builder.Services.AddUserJwtManager(jwtOptions);
    builder.Services.Configure<JwtOptions>(jwtOptionsSection);
    // check access token
    builder.Services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add<AccessTokenCheckFilter>();
    });

    builder.Services.AddSingleton<IAuthorizationHandler, HimuContestAuthorizationCrudHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
    // Problem authorization handler used EFCore.
    builder.Services.AddScoped<IAuthorizationHandler, HimuProblemAuthorizationCrudHandler>();

    #endregion Identity Options

    #region Email Service

    builder.Services.AddMailSenderService();
    builder.Services.Configure<MailSenderOptions>(builder.Configuration.GetSection("MailService"));

    #endregion Email Service

    #region Redis Options

    var enableRedis = builder.Configuration.GetValue<bool>("UseRedis");
    // Redis cache enabled
    if (enableRedis)
    {
        var redisOptions = builder.Configuration.GetSection("RedisCacheOptions");
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration
                = (string?) redisOptions.GetValue(typeof(string), "Configuration");
            options.InstanceName
                = (string?) redisOptions.GetValue(typeof(string), "InstanceName");
        });
    }
    // Memory cache
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }

    #endregion Redis Options

    #region WebApi Options

    builder.Services.AddControllers().AddControllersAsServices();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSignalR();

    #endregion WebApi Options

    #region Swagger Options

    builder.Services.AddSwaggerGen(c =>
    {
        var scheme = new OpenApiSecurityScheme
        {
            // ReSharper disable once StringLiteralTypo
            Description = "Input JWT Token: Bearer xxxxxxxx",
            Reference
                = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Authorization" },
            Scheme = "oauth2",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        };

        c.AddSecurityDefinition("Authorization", scheme);
        var requirement = new OpenApiSecurityRequirement
        {
            [scheme] = new List<string>()
        };
        c.AddSecurityRequirement(requirement);
    });

    #endregion Swagger Options

    #region ImageSharp

    builder.Services.AddImageSharp();

    #endregion ImageSharp

    #region Cors

    string[] urls = new string[]
    {
        "http://localhost:5173"
    };

    builder.Services.AddCors(options => options.AddDefaultPolicy(
        policyBuilder => policyBuilder.WithOrigins(urls)
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod())
    );

    #endregion Cors

    #region Judge Services
    builder.Services.AddHimuJudgeCoreServices();
    #endregion
    // Build

    #region Build App

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseHttpsRedirection();
    app.UseImageSharp();
    app.UseStaticFiles();
    // Serilog request & response
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHub<JudgeHub>("/judgehub");
    app.MapControllers();
    app.Run();

    #endregion Build App
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly X_X");
}
finally
{
    Log.CloseAndFlush();
}