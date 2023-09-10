using Blazored.LocalStorage;
using GLSPM.Client.Services;
using GLSPM.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace GLSPM.Client
{
    public static class Configurations
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services, IWebAssemblyHostEnvironment environment)
        {
            var client = new HttpClient { BaseAddress = new Uri(environment.BaseAddress) };
            services
                .AddSingleton(client)
                .AddMudServices()
                .AddBlazoredLocalStorage()
                .AddAuthorizationCore()
                .AddScoped<GLSPMAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p=>p.GetRequiredService<GLSPMAuthenticationStateProvider>())
                .AddScoped<IAccountsService, AccountsService>()
                .AddScoped<IPasswordsService, PasswordsService>();

            return services;
        }
    }
}
