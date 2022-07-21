using GClaims.Core.Extensions;
using GClaims.HangFire.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.BuildSettings();

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

