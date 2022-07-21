using GClaims.Core.Extensions;
using GClaims.Host;

var builder = WebApplication.CreateBuilder(args);

builder.BuildSettings(true);

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