using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Requests;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Validators.AccountValidators;

namespace GClaims.Marvel.Application.Accounts.Comands;

public class CreateAccountCommand : Command<CreateAccountCommand, CreateAccountResponse, CreateAccountValidator,
    MarvelAccountDto>
{
    public CreateAccountCommand(CreateAccountRequest request)
    {
        Request = request;
    }

    public CreateAccountRequest Request { get; }
}