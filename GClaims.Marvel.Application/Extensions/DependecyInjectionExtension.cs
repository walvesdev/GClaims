using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts;
using GClaims.Marvel.Application.Accounts.Comands;
using GClaims.Marvel.Application.Accounts.Queries;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Infrastructure.Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Marvel.Application.Extensions;

public static class DependecyInjectionExtension
{
    public static IServiceCollection AddMarvelDependency(this IServiceCollection services,
        IConfiguration configuration)
    {
        // ====================================
        // Contexts Cofig
        // ====================================

        services.AddScoped<DbSession>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<MarverAccountRepository>();

        // ====================================
        // Commands
        // ====================================

        // Account
        services.AddScoped<ICommandHandler<CreateAccountCommand, long>, AccountCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateAccountCommand, bool>, AccountCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAccountCommand, bool>, AccountCommandHandler>();

        // ====================================
        // Queries
        // ====================================

        // Account
        services.AddScoped<IQueryHandler<GetAllAccountQuery, GetAllAccountResponse>, AccountQueryHandler>();

        return services;
    }
}