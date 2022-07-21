using GClaims.HangFire.Host;
using GClaims.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureHostConfiguration(webBuilder =>
    {
        var env = builder.Environment;
        var root = GetRootDirectory();

        webBuilder.AddCertificates(root);

        webBuilder.AddJsonFile(Path.Combine(root, "appsettings.json"), false, true);

        //Project Folder
        if (env.IsDevelopment() || env.EnvironmentName.Contains("Dev."))
        {
            webBuilder
                .AddJsonFile(Path.Combine(root, "..", "CommonSettings.json"), false, true)
                .AddJsonFile(Path.Combine(root, "..", $"CommonSettings.{env.EnvironmentName}.json"), false,
                    true);

            env.EnvironmentName = "Development";
        }
        else
        {
            webBuilder
                .AddJsonFile(Path.Combine(root, "CommonSettings.json"), false, true)
                .AddJsonFile(Path.Combine(root, $"CommonSettings.{env.EnvironmentName}.json"), false, true);
        }

        webBuilder.AddEnvironmentVariables();
    });

builder.WebHost
    .UseSockets()
    .UseKestrel(so =>
    {
        so.Limits.MaxConcurrentConnections = 100;
        so.Limits.MaxConcurrentUpgradedConnections = 100;
        so.Limits.MaxRequestBodySize = 52428800;
    });


var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, builder.Environment);
// startup.ConfigureEndpoints(app, app.Services);

app.Run();

string GetRootDirectory()
{
    var rootDirectory = AppContext.BaseDirectory;
    if (rootDirectory.Contains("bin"))
    {
        rootDirectory = rootDirectory[..rootDirectory.IndexOf("bin", StringComparison.Ordinal)];
    }

    return rootDirectory;
}