using GClaims.BuildingBlocks.Core.Common;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;

namespace GClaims.Marvel.Application.Accounts.Responses;

public class GetAllAccountResponse : InputQueryResponse<MarvelAccountDto>
{
    public GetAllAccountResponse(ICommandQuery query, ICollection<MarvelAccountDto> list) : base(query, list)
    {
    }
}