using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netcore.Core.Configurations;

namespace netcore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

    public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // we have to place this configuration before initializing component
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    // set up configuration before other component initializes
                    var env = hostingContext.HostingEnvironment;
                    var contentRootPath = hostingContext.HostingEnvironment.ContentRootPath;
                    var basePath = env.IsDevelopment() ? Path.Combine(contentRootPath, "Resources", env.EnvironmentName) : contentRootPath;

                    builder
                        // set builder base path for reading appsettings file
                        .SetBasePath(basePath)
                        // add appsettings file for read
                        .AddJsonFile("appsettings.json", true, true);
                })
                .UseStartup<Startup>()
                .UseKestrel(options => {
                    // get services
                    var env = options.ApplicationServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
                    var server = (options.ApplicationServices.GetService(typeof(AppSettings)) as AppSettings).Server;
                    // configure server with https
                    if (server.UseHttps) 
                    {
                        var certPath = env.IsDevelopment() ? 
                            Path.Combine(env.ContentRootPath, "Resources", env.EnvironmentName, server.Certificate.CertName)
                            : server.Certificate.CertName;
                        // configuration server with ssl cert and ssl port
                        options.Listen(IPAddress.Any, server.SSLPort, listenOptions => {
                            listenOptions.UseHttps(certPath, server.Certificate.Password);
                        });
                    }
                    // Add listening options for each address
                    options.Listen(IPAddress.Any, server.Port);
                })
                .Build();
    }
}
