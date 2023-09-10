using FluentValidation.AspNetCore;
using GLSPM.Application.EFCore;
using GLSPM.Application.EFCore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CubesFramework.Security;
using System.Security.Cryptography;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.AppServices;
using GLSPM.Domain;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using GLSPM.Application.Dtos;
using Microsoft.IdentityModel.Tokens;
using GLSPM.Domain.Dtos;

namespace GLSPM.Application
{
    public static class Configurations
    {
        public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddHttpContextAccessor()
                    .ConfigureDB(configuration)
                    .ConfigEFCoreLayer()
                    .ConfigureIdentity()
                    .ConfigureJwt(configuration)
                    .ConfigureFV()
                    .ComfigureCubesFW()
                    .Configure<FilesPathes>(configuration.GetSection("FilesPathes"))
                    .ConfigureUriServices()
                    .AddAutoMapper(typeof(GLSPMMappingProfile))
                    .ConfigureAppSerivces();
            return services;
        }
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                //lockout
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 5, 0);
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<GLSPMDBContext>()
            .AddDefaultTokenProviders();
            return services;
        }
        public static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtsettings = configuration.GetSection("Jwt");
            var secretkey = jwtsettings.GetSection("Key").Value;
            //using the bearer auth scheme
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //setting the jwt bearer token options
            .AddJwtBearer(o =>
            {
                //configure response
                o.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        //to skip the default logic and avoid using the default response
                        context.HandleResponse();
                        //custom response 
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new SingleObjectResponse<object>
                        {
                            Success = false,
                            Message = "User is not authorized",
                            StatusCode = 401,
                            Error = new object[] { new { Message = "User is not authorized" } }
                        });
                    }
                };
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    SaveSigninToken = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtsettings.GetSection("Issuer").Value,
                    ValidAudience = jwtsettings.GetSection("Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            //registreation of the authentication service
            services.AddScoped<IAuthenticationAppService,AuthenticationAppService>();

            return services;
        }
        public static IServiceCollection ConfigureFV(this IServiceCollection services)
        {
            services.AddControllers()
               .AddFluentValidation(config =>
               {
                   config.DisableDataAnnotationsValidation = true;
                   config.RegisterValidatorsFromAssembly(Assembly.GetCallingAssembly());
               });
            return services;
        }
        public static IServiceCollection ComfigureCubesFW(this IServiceCollection services)
        {
            var crypto = new Crypto(SHA256.Create());
            services.AddSingleton(crypto);
            return services;
        }
        public static IServiceCollection ConfigureDB(this IServiceCollection services, IConfiguration configuration)
        {
            var MSCS = configuration.GetConnectionString("MSCS");
            var MYSCS = configuration.GetConnectionString("MYSCS");
            var liteCS = configuration.GetConnectionString("liteCS");
           
            services.AddDbContext<GLSPMDBContext>(options =>
            {
                if (!string.IsNullOrWhiteSpace(liteCS))
                options.UseSqlite(liteCS, c => c.MigrationsAssembly("GLSPM.Server"));
                else if (!string.IsNullOrWhiteSpace(MSCS))
                    options.UseSqlServer(MSCS, c => c.MigrationsAssembly("GLSPM.Server"));
                else if (!string.IsNullOrWhiteSpace(MYSCS))
                    options.UseMySql(connectionString: MYSCS, serverVersion:ServerVersion.AutoDetect(MYSCS), c => c.MigrationsAssembly("GLSPM.Server"));

                options.EnableDetailedErrors();
            });
            return services;
        }
        public static IServiceCollection ConfigureAppSerivces(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppService<,,,,>), typeof(AppServiceBase<,,,,>));
            services.AddScoped<ICardsAppService,CardAppSerivce>();
            services.AddScoped<IPasswordsAppService, PasswordAppService>();
            return services;
        }
        public static IServiceCollection ConfigureUriServices(this IServiceCollection services)
        {
            services.AddSingleton<IUriAppService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriAppService(uri);
            });
            return services;
        }
    }
}
