using GClaims.BuildingBlocks.Core.Common;
using GClaims.Marvel.Application.Accounts.Dtos;

namespace GClaims.Marvel.Application.Accounts.Requests;

public class UpdateAccountRequest : IInputRequest<MarvelAccountDto>
{
    public MarvelAccountDto? Input { get; set; }
}