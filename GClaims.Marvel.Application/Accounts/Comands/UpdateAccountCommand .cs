using GClaims.BuildingBlocks.Core.Common;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Requests;
using GClaims.Marvel.Application.Validators.AccountValidators;

namespace GClaims.Marvel.Application.Accounts.Comands;

public class UpdateAccountCommand : Command<UpdateAccountCommand, bool, UpdateAccountValidator, MarvelAccountDto>,
    IInputRequest<MarvelAccountDto>
{
    public UpdateAccountCommand(UpdateAccountRequest request)
    {
        Input = request.Input;
    }

    public MarvelAccountDto? Input { get; set; }
}