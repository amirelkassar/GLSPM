global using static GLSPM.Domain.ApplicationConses;
global using static GLSPM.Domain.ApplicationConses.ClientConses;
global using static GLSPM.Domain.ApplicationConses.Apis;
global using GLSPM.Domain.Dtos.Identity;
global using GLSPM.Domain.Dtos.Passwords;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GLSPM.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddClientServices(builder.HostEnvironment);

await builder.Build().RunAsync();
