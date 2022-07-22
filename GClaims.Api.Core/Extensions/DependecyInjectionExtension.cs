using GClaims.BuildingBlocks.Application.Mediator;
using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.BuildingBlocks.Infrastructure.EventSourcing;
using GClaims.Core.Auth;
using GClaims.Core.Services.Http;
using GClaims.Domain.Models.Auth.Users;
using GClaims.Marvel.Application.Accounts;
using GClaims.Marvel.Application.Accounts.Comands;
using GClaims.Marvel.Application.Accounts.Queries;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Extensions;

public static class DependecyInjectionExtension
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<LoginService>();
        services.AddScoped<SigningConfigurations>();
        services.AddScoped<TokenConfigurations>();
        services.AddScoped<AppUser>();
        services.AddScoped<HttpClient>();
        services.AddScoped(typeof(HttpClientService<,,,>));
        

        // ====================================
        // Contexts Cofig
        // ====================================

        // Context
        // services.AddDbContext<DbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("Default")));

        // ====================================
        // Events Config
        // ====================================

        // Mediator
        services.AddScoped<IMediatorHandler, MediatorHandler>();

        // Notifications
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        // Events
        // services.AddScoped<INotificationHandler<Event>, EventHandler>();

        // Event Sourcing
        services.AddSingleton<IEventStoreService, EventStoreService>();
        services.AddSingleton<IEventSourcingRepository, EventSourcingRepository>();

        // ====================================
        // Modules
        // ====================================

        services.AddMarvelDependency(configuration);

        return services;
    }
}