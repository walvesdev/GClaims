using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Queries;
using GClaims.Marvel.Application.Accounts.Responses;

namespace GClaims.Marvel.Application.Accounts;

public class AccountQueryHandler : IQueryHandler<GetAllAccountQuery, GetAllAccountResponse>
{
    public AccountQueryHandler(IMediatorHandler mediatorHandler)
    {
        MediatorHandler = mediatorHandler;
    }

    public IMediatorHandler MediatorHandler { get; set; }

    public async Task<GetAllAccountResponse> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
    {
        // return new GetAllAccountResponse(request,
        //     (await Repository.GetAllListAsync()).Map<List<MarvelAccount>, List<MarvelAccountDto>>());
        return new GetAllAccountResponse(request, new List<MarvelAccountDto>());
    }
}