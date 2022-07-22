using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using GClaims.Core;
using GClaims.Core.Auth;
using GClaims.Core.Extensions;
using GClaims.Core.FIlters;
using GClaims.Core.Filters.CustomExceptions;
using GClaims.Core.Middlewares;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Core.Models;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace GClaims.Host;

public class Startup
{
    private const string CORS_NAME = "AllowAll";
#if DEBUG
    private const int CACHE_TIME = 0; //Segundos
#else
        const int CACHE_TIME = 120; //Segundos
#endif
    public Startup(IConfigurationRoot configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfigurationRoot Configuration { get; }

    public IWebHostEnvironment Environment { get; set; }

    public ILogger Logger { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        var appSettings = Configuration.GetSection("AppSettings").Get<AppSettingsSection>();

        // Add services to the container.
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddDependencyInjectionByConvention(typeof(Program).GetTypeInfo().Assembly);

        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<MarvelAccount, MarvelAccountDto>();
            cfg.CreateMap<MarvelAccountDto, MarvelAccount>();
        });
        var mapper = config.CreateMapper();

        services.AddSingleton(mapper);

        services.AddDependencyInjection(Configuration);

        services.AddCors(options =>
        {
            options.AddPolicy(CORS_NAME, builder =>
            {
                builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(appSettings.Security.CorsOrigins.ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddOptions();
        services.AddResponseCaching();
        services.AddSignalR();

        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations(true, true);
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GClaims API",
                Version = "v1"
            });

            swagger.CustomSchemaIds(x => x.FullName);
            swagger.DescribeAllParametersInCamelCase();

            var XMLPath = AppDomain.CurrentDomain.BaseDirectory + "GClaims.Api.Host" + ".xml";
            if (File.Exists(XMLPath))
            {
                swagger.IncludeXmlComments(XMLPath);
            }

            var basePath = AppContext.BaseDirectory;

            Directory.GetFiles($"{basePath}/", "*.xml").ToList().ForEach(file =>
            {
                swagger.IncludeXmlComments(file, true);
            });

            swagger.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });

            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] { }
                }
            });
        });

        JsonConvert.DefaultSettings = () =>
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        };
        
        services.AddHealthChecks();
        
        services.AddHealthChecks()
            .AddSqlServer(Configuration.GetConnectionString("Default"),
                name: "sqlserver", tags: new string[] { "db", "data" });
        
        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddRazorPages();
        services.AddControllers(opt =>
            {
                opt.AllowEmptyInputInBodyModelBinding = true;
                opt.Filters.Add(typeof(CustomExceptionFilter));
                opt.Filters.Add(typeof(ModelStateValidationFilter));
                opt.RespectBrowserAcceptHeader = true;
            })
            .ConfigureApiBehaviorOptions(o => { o.SuppressModelStateInvalidFilter = true; })
            .AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
            .AddNewtonsoftJson(opts =>
            {
                var settings = JsonConvert.DefaultSettings();

                opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                opts.SerializerSettings.DefaultValueHandling = settings.DefaultValueHandling;

                foreach (var converter in settings.Converters)
                {
                    opts.SerializerSettings.Converters.Add(converter);
                }

                opts.SerializerSettings.Formatting = Formatting.Indented;
            });

        services.AddJwtConfigWebApi(Configuration);

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
    }

    public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime, ILogger logger)
    {
        Logger = logger;

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        app.UseForwardedHeaders();

        app.Use((context, next) =>
        {
            if (!Environment.IsDevelopment())
            {
                context.Request.Scheme = "https";
            }

            return next();
        });

        // Configure the HTTP request pipeline.
        if (Environment.IsDevelopment())
        {
            app.UseMiddleware(typeof(RequestLogMiddleware));
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Use(async (context, next) =>
            {
                var excHandler = context.Features.Get<IExceptionHandlerFeature>();
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                var response = JsonConvert.SerializeObject(excHandler?.Error, Formatting.Indented);
                logger.LogError(response);
                await context.Response.WriteAsync(response, Encoding.UTF8);
            }, true);
        });

        app.UseSwagger();
        app.UseSwaggerUI(ui =>
        {
            ui.SwaggerEndpoint("v1/swagger.json", "GClaims API v1");
            ui.DocExpansion(DocExpansion.None);
        });
        
        app.UseHealthChecks("/status",
            new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            statusApplication = report.Status.ToString(),
                            healthChecks = report.Entries.Select(e => new
                            {
                                check = e.Key,
                                status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            })
                        });

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

        // Generated the endpoint which will return the needed data
        app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Activate the dashboard for UI
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/monitor";
        });


        app.UseRouting();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors(CORS_NAME);
        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}