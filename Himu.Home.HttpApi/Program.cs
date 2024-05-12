// Copyright(c) 2023 Himu (himu.liuhuan@gmail.com)

using Himu.Common.Service;
using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Himu.HttpApi.Utility.Authorization;
using Himu.HttpApi.Utility.Utility;
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

    #endregion

    // Add services to the container.

    #region Filters

    builder.Services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add<RecordBadApiResponseFilter>();
    });

    #endregion

    #region Validation

    builder.Services.AddHimuApiValidation();

    #endregion

    #region EFCore Options

    builder.Services.AddDbContext<HimuMySqlContext>(options =>
    {
        string? connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
        Debug.Assert(connectionString != null);
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });

    #endregion

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
    identityBuilder.AddEntityFrameworkStores<HimuMySqlContext>()
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
    // Problem authorization handler used EFCore.
    builder.Services.AddScoped<IAuthorizationHandler, HimuProblemAuthorizationCrudHandler>();
    #endregion

    #region Email Service

    builder.Services.AddMailSenderService();
    builder.Services.Configure<MailSenderOptions>(builder.Configuration.GetSection("MailService"));

    #endregion

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

    #endregion

    #region WebApi Options

    builder.Services.AddControllers().AddControllersAsServices();
    builder.Services.AddEndpointsApiExplorer();

    #endregion

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

    #endregion

    #region Judge Core Service

    builder.Services.AddJudgeCoreService();
    builder.Services.Configure<JudgeCoreConfiguration>(
        builder.Configuration.GetSection("JudgeCore"));

    #endregion

    #region ImageSharp

    builder.Services.AddImageSharp();

    #endregion

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
    app.MapControllers();
    app.Run();

    #endregion
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly X_X");
}
finally
{
    Log.CloseAndFlush();
}