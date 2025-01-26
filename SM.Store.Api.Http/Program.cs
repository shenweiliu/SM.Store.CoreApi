using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SM.Store.Api.DAL;
using SM.Store.Api.HostShared;

namespace SM.Store.Api.Http
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Enable below code section for using Http.sys hosting.
            builder.WebHost.UseHttpSys(options =>
            {
                options.AllowSynchronousIO = false;
                options.Authentication.Schemes = AuthenticationSchemes.None; //Default setting
                //options.Authentication.Schemes = AuthenticationSchemes.Basic; //Enable this to enforce auth
                options.Authentication.AllowAnonymous = true;
                options.MaxConnections = null;
                options.MaxRequestBodySize = 30_000_000;

                //Won't work if using setting here without applicationUrl in launchSettings.json
                //options.UrlPrefixes.Add("http://localhost:5024");
                //options.UrlPrefixes.Add("https://localhost:44360");
            });
            
            builder.Logging.ClearProviders()
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug()
                .AddEventSourceLogger();

            //Add scoped for data context for initializing database if not exists.
            builder.Services.AddScoped<StoreDataContext>();

            var startup = new Startup(builder.Environment, TargetType.HTTP_Sys);
            startup.ConfigureServices(builder.Services);
            
            var app = builder.Build();
            startup.Configure(app);

            if (!builder.Environment.IsProduction())
            {
                //Initialize database if not exists.
                //Add AttachDBFilename=|DataDirectory|\*.mdf into ConnectionString if not use default db file location for SQL LocalDB. 
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var storeDataContext = services.GetRequiredService<StoreDataContext>();
                    StoreDataInitializer.Initialize(storeDataContext);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            app.Run();
        }        
    }
}
