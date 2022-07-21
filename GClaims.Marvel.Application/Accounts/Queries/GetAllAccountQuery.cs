using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Requests;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Validators.AccountValidators;

namespace GClaims.Marvel.Application.Accounts.Queries;

public class GetAllAccountQuery :
    Query<GetAllAccountQuery, GetAllAccountResponse, GetAllAccountValidator, MarvelAccountDto>
{
    public GetAllAccountQuery(GetAllAccountRequest request) : base(request)
    {
    }
}