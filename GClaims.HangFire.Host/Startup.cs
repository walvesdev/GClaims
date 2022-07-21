using System.Reflection;
using DinkToPdf;
using DinkToPdf.Contracts;
using GClaims.HangFire.Core.Filters;
using GClaims.Core;
using GClaims.Core.Extensions;
using GClaims.Core.Filters.CustomExceptions;
using GClaims.Core.Resolvers;
using Hangfire;
using Hangfire.Console;
using Hangfire.Redis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace GClaims.HangFire.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppServices.ServiceCollection = services;
            AppServices.Configuration = Configuration;

            var hangfireAssembly = typeof(Startup).GetTypeInfo().Assembly;
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSignalR();
            services.AddDependencyInjection(Configuration);
            services.AddDependencyInjectionByConvention(hangfireAssembly);

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettingsSection>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            var cs = Configuration.GetConnectionString("Hangfire");
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(cs));
            
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

#if DEBUG
            services.AddHangfireServer(options =>
            {
                options.ServerName = Environment.MachineName;
                options.Queues = new[] { MachineQueueAttribute.MachineQueueName, "default" };
                options.TimeZoneResolver = new TimeZoneConverterResolver();
            });

            GlobalJobFilters.Filters.Add(new MachineQueueAttribute());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            GlobalConfiguration.Configuration.UseFilter(new MachineQueueAttribute());
            GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
#else
            services.AddHangfireServer(options =>
            {
                options.ServerName = Environment.MachineName;
                options.TimeZoneResolver = new Hangfire.TimeZoneConverterResolver();
            });
#endif

            //services.AddDataProtectionAndRedis(appSettings);
            services.AddControllers(opt => opt.Filters.Add(typeof(CustomExceptionFilter)));
            services.Configure<AppSettingsSection>(Configuration.GetSection("AppSettings"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettingsSection>();

            app.UseRouting();

#if !DEBUG
            app.UseAuthentication();
            app.UseHangfireDashboard(pathMatch: "/dashboard", options: new DashboardOptions
            {
                Authorization = new[] { new CustomAuthorizationFilter(appSettings.LoginUrl, appSettings.CookieName) },
                AppPath = appSettings.LoginUrl,
                DisplayStorageConnectionString = false,
                TimeZoneResolver = new TimeZoneConverterResolver()
            });
#else
            app.UseHangfireDashboard(pathMatch: "/dashboard");
#endif

            app.UseWelcomePage("/");

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}