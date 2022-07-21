using GClaims.Core.Extensions;
using GClaims.Host;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureLogging(logging => { logging.AddLog4Net(new Log4NetProviderOptions("log4net.config")); })
    .ConfigureHostConfiguration(webBuilder =>
    {
        var env = builder.Environment;
        var root = Directory.GetCurrentDirectory();

        webBuilder.AddCertificates(root);

        webBuilder.AddJsonFile(Path.Combine(root, "appsettings.json"), false,
                true)
            .AddJsonFile(Path.Combine(root, $"appsettings.{env.EnvironmentName}.json"), false,
                true);

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

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Lifetime, app.Logger);

app.Run();