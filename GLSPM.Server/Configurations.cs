using GLSPM.Application;
using GLSPM.Application.EFCore;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace GLSPM.Server
{
    public static class Configurations
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.AddSerilog();
            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
                                options.SerializerSettings.ReferenceLoopHandling
                                = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationLayer(builder.Configuration, builder.Environment);

            builder.Services.AddSwaggerGen(config =>
            {
                string description = " Password Manager WEB apis";
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "GLSPM", Version = "v1", Description = description });
            });
            return builder;
        }
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            return builder;
        }

        public static WebApplication Setup(this WebApplication app)
        {
            app.AddGlobalExceptionHandler();
            //seed the admin user
            using (var scope = app.Services.CreateScope())
            {
                var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                userManager.SeedDefUsers();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            //blazor
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            //routing
            app.UseRouting();
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
            //auth
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");
            return app;
        }
        public static WebApplication AddGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var httperror = context.Features.Get<IExceptionHandlerFeature>();
                    if (httperror != null)
                    {
                        Log.Error($"An application error occured {httperror.Error}");
                        await context.Response.WriteAsJsonAsync(new SingleObjectResponse<object>
                        {
                            Success = false,
                            Message = $"An application error occured {httperror.Error}",
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Error = new object[] { new { Message = $"An application error occured {httperror.Error}" } }
                        });
                    }
                });
            });
            return app;
        }
        public static UserManager<ApplicationUser> SeedDefUsers(this UserManager<ApplicationUser> userManager)
        {
            var admin = new ApplicationUser
            {
                Id = "1",
                UserName = "Admin",
                NormalizedUserName = "Admin".ToUpper(),
                Email = "Admin@GoodLawSoftware.com",
                NormalizedEmail = "Admin@GoodLawSoftware.com".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "201120797422",
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
                ImagePath= Path.GetFullPath("./files/imgs/userimg.png") 
            };
            var password = "Admin@2022";
            if (userManager.FindByIdAsync("1").Result==null)
            {
                if (userManager.CreateAsync(admin, password).Result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }

            return userManager;
        }
    }
}
