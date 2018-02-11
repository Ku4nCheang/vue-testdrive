using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using netcore.Core.Authentications;
using netcore.Core.Configurations;
using netcore.Core.Utilities;
using netcore.Models.Contexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace netcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _Env = env;
            // bind the configuration into _appsettings
            _AppSettings = new AppSettings();
            // bind settings
            configuration.Bind(_AppSettings);
        }

        public bool IsDevelopment { get; }
        private AppSettings _AppSettings { get; set; }
        private IHostingEnvironment _Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //
            // ─── CONFIGURATION ───────────────────────────────────────────────
            //

            services.AddSingleton<AppSettings>(_AppSettings);


            //
            // ─── DATA STORE SERVICES ─────────────────────────────────────────
            //

            // non-distributed memory cache
            services.AddMemoryCache();
            // addd redis distributed cache
            // services.AddDistributedRedisCache((opts) => {
            //     opts.ConnectionString = RootConfig.DB.Redis;
            //     opts.Database = 1;
            // });

            if (!this.IsDevelopment)
            {
                // Using sql server database
                services.AddDbContext<ApplicationContext>(options => {
                    options.UseSqlServer(_AppSettings.Database.SQLServer);
                    options.UseLoggerFactory(null);
                });
            }
            else
            {
                // Using in-memory as database, good for development to
                // reset seed data every time when you run the application
                services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("dev"));
            }

            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            //
            // ─── SERVER RELATED FEATURES ─────────────────────────────────────
            //

            // create a jwt options snapshot for controller to create a jwt token.
            services.Configure<JwtBearerOptions>(options => {
                options.Audience = _AppSettings.Authenticate.JwtBearer.Audience;
                options.ClaimsIssuer = _AppSettings.Authenticate.JwtBearer.Issuer;
                options.TokenValidationParameters.IssuerSigningKey = JwtSecurityKey.Create(_AppSettings.Authenticate.JwtBearer.Secret);
            });
             // setting gzip if run on self-container
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        // we need a service provider to provide registered service
                        var provider = services.BuildServiceProvider();
                        options.TokenValidationParameters = new TokenValidationParameters{
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = _AppSettings.Authenticate.JwtBearer.Issuer,
                            ValidAudience = _AppSettings.Authenticate.JwtBearer.Audience,
                            IssuerSigningKey = JwtSecurityKey.Create(_AppSettings.Authenticate.JwtBearer.Secret),
                            RequireExpirationTime = false,
                            SaveSigninToken = true,
                            LifetimeValidator = new CustomLifetimeValidator(provider).ValidateAsync
                        };
                    });

            // setting gzip if run on self-container
            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
            
            if (!_Env.IsDevelopment() && _AppSettings.Server.UseHttps) {
                // enforcing ssl using https
                services.Configure<MvcOptions>(options => {
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }

            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // make sure logger for the service provider 
            // has added a file log
            loggerFactory.AddFile("Logs/logs-{Date}.txt", levelOverrides: new Dictionary<string, LogLevel> {
                { "Default", LogLevel.Information },
                { "Microsoft", LogLevel.Warning },
                { "System", LogLevel.Warning }
            });

            if (_Env.IsDevelopment())
            {
                // hot reload for webpack
                // since wwwroot is place outside the server, we need to set project path
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true,
                    ProjectPath = Directory.GetParent(_Env.ContentRootPath).FullName
                });
            }
            else if (_AppSettings.Server.UseHttps)
            {
                // add url rewriter to make sure only access server via https if use https
                var options = new RewriteOptions().AddRedirectToHttps();
                app.UseRewriter(options);
            }

            app.UseResponseCompression();
            // update web root file provider in order to allow append file version for tag helper.
            var webRootPath = _Env.IsDevelopment() ? Directory.GetParent(_Env.ContentRootPath).FullName : _Env.ContentRootPath;
            var provider = new PhysicalFileProvider(Path.Combine(webRootPath, "wwwroot"));
            _Env.WebRootFileProvider = provider;
            app.UseStaticFiles(new StaticFileOptions  {
                // tell the browser to cache all static files.
                FileProvider = provider,
                RequestPath = "",
                OnPrepareResponse = ctx =>{
                    const int durationInSeconds = 3600 * 24 * 30;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
