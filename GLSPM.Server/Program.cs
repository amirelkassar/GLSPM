using GLSPM.Application;
using GLSPM.Application.EFCore.Repositories;
using GLSPM.Domain.Entities;
using GLSPM.Server;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();
try
{
    Log.Information("Application Starting.");
    var app = builder.Build();
    app.Setup();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
    File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "AppError.txt"), ex.Message);
}
finally
{
    Log.CloseAndFlush();
}


