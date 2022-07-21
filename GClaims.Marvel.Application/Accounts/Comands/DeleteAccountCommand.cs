using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Validators.AccountValidators;

namespace GClaims.Marvel.Application.Accounts.Comands;

public class DeleteAccountCommand : Command<DeleteAccountCommand, bool, DeleteAccountValidator, MarvelAccountDto>
{
    public DeleteAccountCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}