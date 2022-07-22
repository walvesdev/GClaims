using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Core.Extensions;
using GClaims.Marvel.Application.Accounts.Dtos;
using GClaims.Marvel.Application.Accounts.Queries;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Core.Models;
using GClaims.Marvel.Infrastructure.Dapper;

namespace GClaims.Marvel.Application.Accounts;

public class AccountQueryHandler : IQueryHandler<GetAllAccountQuery, GetAllAccountResponse>
{
    public AccountQueryHandler(IMediatorHandler mediatorHandler, MarverAccountRepository repository)
    {
        MediatorHandler = mediatorHandler;
        Repository = repository;
    }

    public MarverAccountRepository Repository { get; }

    public IMediatorHandler MediatorHandler { get; set; }

    public async Task<GetAllAccountResponse> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
    {
        return new GetAllAccountResponse(request,
            (await Repository.GetAll()).ToList().Map<List<MarvelAccount>, List<MarvelAccountDto>>()!);
    }
}