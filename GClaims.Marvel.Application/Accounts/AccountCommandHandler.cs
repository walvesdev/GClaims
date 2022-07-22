using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.IntegrationEvents;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.Core;
using GClaims.Core.Extensions;
using GClaims.Marvel.Application.Accounts.Comands;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Validators.AccountValidators;
using GClaims.Marvel.Core.Models;
using GClaims.Marvel.Infrastructure.Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GClaims.Marvel.Application.Accounts;

public class AccountCommandHandler :
    ICommandHandler<CreateAccountCommand, long>,
    ICommandHandler<DeleteAccountCommand, bool>,
    ICommandHandler<UpdateAccountCommand, bool>
{
    public AccountCommandHandler(
        IMediatorHandler mediatorHandler,
        IOptions<AppSettingsSection> options,
        MarverAccountRepository repository)
    {
        MediatorHandler = mediatorHandler;
        Repository = repository;
        AppSettings = options.Value;

    }

    protected AppSettingsSection AppSettings { get; set; }

    public IMediatorHandler? MediatorHandler { get; set; }

    public MarverAccountRepository Repository { get; }

    public IOptions<AppSettingsSection> Options { get; }

    public async Task<long> Handle(CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var isValid = await MediatorHandler!.Validate<CreateAccountCommand, CreateAccountValidator>(command);

        if (isValid)
        {
            var account = command.Request.Input!.Map<MarvelAccountDto, MarvelAccount>();
            
            await MediatorHandler.PublishEvent(new MarvelAccountIntegrationEvent
            {
                AggregateId = command.Request.Input!.Name,
                Data = JsonConvert.SerializeObject(account)
            });
            
            
            await MediatorHandler.PublishNotification(new DomainNotification("DomainNotification Teste", "Erro DomainNotification")
            {
                Data = new
                {
                    ErroTeste = 2005
                }
            });
            
            return  await Repository.Save(account!);
        }

        return 0;

    }

    public async Task<bool> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var isValid = await MediatorHandler!.Validate<DeleteAccountCommand, DeleteAccountValidator>(command);

        if (isValid)
        {
            var obj = await Repository.Find(command.Id);
            await Repository.Delete(obj);
        }

        return isValid;
    }

    public async Task<bool> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var isValid = await MediatorHandler!.Validate<UpdateAccountCommand, UpdateAccountValidator>(command);

        if (!isValid)
        {
            return isValid;
        }

        await Repository.Update(command.Input!.Map<MarvelAccountDto, MarvelAccount>()!);

        return isValid;
    }
}

public class MarvelAccountIntegrationEvent : IntegrationEvent
{
}