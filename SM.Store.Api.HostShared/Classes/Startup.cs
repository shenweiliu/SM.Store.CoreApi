using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SM.Store.Api.DAL;
using SM.Store.Api.BLL;
using SM.Store.Api.Common;
using SM.Store.Api.Contracts.Configurations;
using SM.Store.Api.Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.IIS;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

namespace SM.Store.Api.HostShared
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, TargetType type)
        {
            WebHostEnvironment = env;
            TargetType = type;
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = configBuilder.Build();
        }

        internal IWebHostEnvironment WebHostEnvironment { get; set; }
        internal TargetType TargetType { get; set; }
        internal IConfiguration Configuration { get; set; }


        public void ConfigureServices(IServiceCollection services)
        {
            //test
            //var ta = StaticConfigs.GetConfig("TestConfig1");

            //Custom IserviceCollection extension for transforming AppConfig items.
            services.TransformAppConfig(WebHostEnvironment);

            //Also make top level configuration available (for EF configuration and access to connection string)
            services.AddSingleton(Configuration); //IConfigurationRoot
            services.AddSingleton<IConfiguration>(Configuration);

            //Add Support for strongly typed Configuration and map to class
            services.AddOptions();
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));

            //Set database.
            //if (Configuration["AppConfig:UseInMemoryDatabase"] == "true")
            if (StaticConfigs.GetConfig("UseInMemoryDatabase") == "true")
            {
                services.AddDbContext<StoreDataContext>(opt => opt.UseInMemoryDatabase("StoreDbMemory"));
            }
            else
            {
                services.AddDbContext<StoreDataContext>(c =>
                    c.UseSqlServer(Configuration.GetConnectionString("StoreDbConnection")));
            }

            //Cors policy is added to controllers via [EnableCors("CorsPolicy")]
            //or .UseCors("CorsPolicy") globally
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //.AllowCredentials() //Core 3.0+ removed.
                        );
            });

            //Instance injection
            services.AddScoped(typeof(IAutoMapConverter<,>), typeof(AutoMapConverter<,>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IStoreLookupRepository<>), typeof(StoreLookupRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ILookupBS, LookupBS>();
            services.AddScoped<IProductBS, ProductBS>();
            services.AddScoped<IContactBS, ContactBS>();
            services.AddScoped<IUserService, UserService>();

            ////Per request injections
            //services.AddScoped<ApiExceptionFilter>();

            //Add framework services.
            services.AddControllers(options =>
            {
                //options.Filters.Add(new ApiExceptionFilter());                
            })
            //Add Microsoft.AspNetCore.Mvc.NewtonsoftJson package and use AddNewtonsoftJson.
            .AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                // Configure a custom converter
                //options.SerializerSettings.Converters.Add(new MyCustomJsonConverter());

                //Handle enum for Swagger - not working for nested enum.
                //options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            //Add Basic Authentication claim transformation.
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            if (TargetType == TargetType.IIS_IISExpress)
            {
                //Register authentication scheme that takes the setting from IIS/IIS Express.
                services.AddAuthentication(IISServerDefaults.AuthenticationScheme);
            }

            //Basic authentication of identity claim type. Not work for later ASP.NET Core versions.
            //services.AddAuthentication("BasicAuthentication")
            //    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            //API basic auth.
            var authRoleGroup = StaticConfigs.GetConfig("AuthRoleGroup") ?? "Basic Auth Role";
            if (!WebHostEnvironment.IsEnvironment("LOCAL"))
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("RoleBasedBasicAuth", policy =>
                        policy.RequireRole(authRoleGroup));
                });
            }
            else
            {
                //Bypass auth for LOCAL environment;
                services.AddAuthorization(options =>
                {                    
                    options.AddPolicy("RoleBasedBasicAuth", policy =>
                       policy.RequireAssertion(ahc => true));
                       //Temp enable for test auth on "LOCAL" environment.
                       //policy.RequireRole(authRoleGroup));
                });
            }

            //Swagger.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Store Data Service API",
                    Version = "v1",
                    Description = "ASP.NET Core data service application",
                });
                //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"                            
                            }
                        },
                        new string[] {}
                    }
                });                
            });

            //Enum support for Swagger - not working for nested enum.
            //services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app)
        {
            //Custom global error handling, logging and reporting.
            app.ConfigureExceptionHandler();

            //app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            //index.html is not required
            //app.UseDefaultFiles();

            //Apply CORS.
            app.UseCors("CorsPolicy");
            
            //Before UseAuthorization.
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //Swagger.
            if (!WebHostEnvironment.IsEnvironment("PROD"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Data Services"));
            }                

            //put last so header configs like CORS or Cookies etc can fire
            //app.UseMvcWithDefaultRoute();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    //Basic Auth claim transformation. 
    public class ClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("now", DateTime.Now.ToString()));
            return Task.FromResult(principal);
        }
    }
}
