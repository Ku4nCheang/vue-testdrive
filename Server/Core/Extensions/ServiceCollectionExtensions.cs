using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using netcore.Core.Configurations;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using netcore.Models.Contexts;
using netcore.Core.Authentications;
using netcore.Core.Utilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using netcore.Models;
using netcore.Models.Mappings;
using System.Net;
using System.Threading.Tasks;
using netcore.Core.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace netcore.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// add related database services into application.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="settings">Database configuration settings</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IServiceCollection that can be used to add services.</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, DatabaseSettings settings)
        {
            // Using sql server database
            services.AddDbContext<ApplicationContext>(options =>
            {
                if (!settings.UseInMemory)
                {
                    options.UseSqlServer(settings.SQLServer);
                    options.UseLoggerFactory(null);
                }
                else
                {
                    // Using in-memory as database, good for development to
                    // reset seed data every time when you run the application
                    options.UseInMemoryDatabase("dev");
                }
            });
            return services;
        }

        /// <summary>
        /// add related redis cache services into application.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="settings">Database configuration settings</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IServiceCollection that can be used to add services.</returns>
        public static IServiceCollection AddRedisCache(this IServiceCollection services, DatabaseSettings settings)
        {
            // // addd redis distributed cache
            // services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>((context) => {
            //     var multiplexer = ConnectionMultiplexer.Connect(settings.Redis);
            //     return multiplexer;
            // });
            // // services.AddScoped<RedisCacheContext>();
            // // Binds the values in action to options and allow to be read by above services
            // services.Configure<RedisConnectionOptions>((options) => {
            //     options.ConnectionString = settings.Redis;
            //     options.Database = 1;
            // });
            return services;
        }

        public static IServiceCollection AddUserIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            var password = settings.Password;
            var user = settings.User;
            var lockout = settings.Lockout;

            services
                .AddIdentity<User, UserRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = password.RequireDigit;
                    options.Password.RequiredLength = password.RequiredLength;
                    options.Password.RequireNonAlphanumeric = password.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = password.RequireUppercase;
                    options.Password.RequireLowercase = password.RequireLowercase;
                    options.Password.RequiredUniqueChars = password.RequiredUniqueChars;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = lockout.DefaultLockoutTimeSpan;
                    options.Lockout.MaxFailedAccessAttempts = lockout.MaxFailedAccessAttempts;
                    options.Lockout.AllowedForNewUsers = lockout.AllowedForNewUsers;

                    // User settings
                    options.User.RequireUniqueEmail = user.RequireUniqueEmail;
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtBearer(this IServiceCollection services, JwtBearerSettings settings)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // we need a service provider to provide registered service
                    var provider = services.BuildServiceProvider();
                    options.Audience = settings.Audience;
                    options.ClaimsIssuer = settings.Issuer;
                    options.Events = new JwtBearerEvents() {
                        OnAuthenticationFailed = ctx => {
                            ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                            return Task.FromResult(0);
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = settings.Issuer,
                        ValidAudience = settings.Audience,
                        IssuerSigningKey = JwtSecurityKey.Create(settings.Secret),
                        RequireExpirationTime = true,
                        SaveSigninToken = true
                        // LifetimeValidator = new CustomLifetimeValidator(provider).ValidateAsync
                    };
                });

            // policy requirements
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("OwnerOrInternalUser", policy => {
                        policy.Requirements.Add(new OwnerOrAnyRoleRequirement("SystemUser,Administrator"));
                    });
                    options.AddPolicy("SameUser", policy =>
                        policy.Requirements.Add(new SameUserRequirement()));
                })
                .AddSingleton<IAuthorizationHandler, OwnerOrAnyRoleHandler>()
                .AddSingleton<IAuthorizationHandler, SameUserHandler>();
            // cross origin policy
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .Build());
            });

            return services;
        }

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddFixture(this IServiceCollection services)
        {
            services.AddTransient<Seed>();
            return services;
        }

        public static IServiceCollection AddMapping(this IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddSingleton<UserProfile>();
            return services;
        }
    }
}