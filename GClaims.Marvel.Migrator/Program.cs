using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();

        IHostEnvironment env = hostingContext.HostingEnvironment;

        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

        IConfigurationRoot configurationRoot = configuration.Build();
    }).ConfigureServices(service =>
    {
        service
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SqlServer support to FluentMigrator
                .AddSqlServer()
                // Set the connection string
                .WithGlobalConnectionString(
                    "Data Source=tcp:localhost,1433;Initial Catalog=Marvel;User Id=sa;Password=1q2w3E*;MultipleActiveResultSets=True;Connect Timeout=300;")
                // Define the assembly containing the migrations
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    })
    .Build();

// Put the database update into a scope to ensure
// that all resources will be disposed.
using (var scope = host.Services.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider);
}

/// <summary>
/// Update the database
/// </summary>
static void UpdateDatabase(IServiceProvider serviceProvider)
{
    // Instantiate the runner
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    try
    {
        // Execute the migrations
        runner.MigrateUp();
    }
    catch (Exception e)
    {
        Console.WriteLine("Falha ao executar Migração! " + e.Message);
        Console.WriteLine("Precine um tecla pra sair!");
        Console.ReadKey();
        Environment.Exit(0);
    }

    Console.WriteLine("Migração executada com sucesso!");
    Console.WriteLine("Precine um tecla pra sair!");
    Console.ReadKey();
    Environment.Exit(0);
}

await host.RunAsync();