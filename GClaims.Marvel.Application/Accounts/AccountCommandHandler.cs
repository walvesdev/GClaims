using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.IntegrationEvents;
using GClaims.Core;
using GClaims.Core.Extensions;
using GClaims.Marvel.Application.Accounts.Comands;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Validators.AccountValidators;
using GClaims.Marvel.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GClaims.Marvel.Application.Accounts;

public class AccountCommandHandler :
    ICommandHandler<CreateAccountCommand, CreateAccountResponse>,
    ICommandHandler<DeleteAccountCommand, bool>,
    ICommandHandler<UpdateAccountCommand, bool>
{
    public AccountCommandHandler(
        IMediatorHandler mediatorHandler,
        IOptions<AppSettingsSection> options)
    {
        MediatorHandler = mediatorHandler;
        AppSettings = options.Value;

    }

    protected AppSettingsSection AppSettings { get; set; }

    public IMediatorHandler? MediatorHandler { get; set; }

    public IOptions<AppSettingsSection> Options { get; }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var isValid = await MediatorHandler!.Validate<CreateAccountCommand, CreateAccountValidator>(command);

        if (isValid)
        {
            var account = command.Request.Input!.Map<MarvelAccountDto, MarvelAccount>();
            
            await MediatorHandler.PublishEvent(new MarvelAccountIntegrationEvent
            {
                AggregateId = command.Request.Input!.Id,
                Data = JsonConvert.SerializeObject(account)
            });
            
            // var result = await Repository.InsertAsync(Account);
            // var response = new CreateAccountResponse().Assign(result);
            //return response;
        }

        return new CreateAccountResponse
        {
            Id = Guid.Parse("c187cf46-4236-4233-85fe-2909a0484ac7"),
            Name = "Teste OK"
        };
    }

    public async Task<bool> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var isValid = await MediatorHandler!.Validate<DeleteAccountCommand, DeleteAccountValidator>(command);

        if (isValid)
        {
            //await Repository.DeleteAsync(command.Id);
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

        //await Repository.UpdateAsync(command.Input.Map<MarvelAccountDto, FinancialAccount>());

        return isValid;
    }
}

public class MarvelAccountIntegrationEvent : IntegrationEvent
{
}