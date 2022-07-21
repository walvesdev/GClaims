#region

using FluentValidation;
using FluentValidation.Results;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using MediatR;

#endregion

namespace GClaims.BuildingBlocks.Core.Messages;

public abstract class Query<TRequest, TResponse> : Command<TRequest, TResponse>, IQueryMessage<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected Query()
    {
        Timestamp = DateTime.Now;
    }

    public string Sorting { get; set; }

    public int CurrentPage { get; set; }

    public int SkipCount { get; set; }

    public int PageSize { get; set; }

    public int MaxResultCount { get; set; }

    public long TotalCount { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < SkipCount;

    public object GetMetadataPagination()
    {
        return new
        {
            MaxResultCount,
            PageSize,
            CurrentPage,
            SkipCount,
            Sorting,
            TotalCount,
            HasNext,
            HasPrevious
        };
    }
}

public abstract class
    Query<TRequest, TResponse, TValidator, TInput> : IQueryMessage<TRequest, TResponse, TValidator, TInput>
    where TRequest : IBaseRequest, IRequest<TResponse>
    where TInput : class, new()
    where TValidator : AbstractValidator<TRequest>, new()
{
    protected Query(ICommandQuery query)
    {
        Timestamp = DateTime.Now;
        CurrentPage = query.CurrentPage;
        SkipCount = query.SkipCount;
        PageSize = query.PageSize;
        MaxResultCount = query.MaxResultCount;
        TotalCount = query.TotalCount;
        Sorting = query.Sorting;
    }

    public int CurrentPage { get; set; }

    public int SkipCount { get; set; }

    public int PageSize { get; set; }

    public int MaxResultCount { get; set; }

    public long TotalCount { get; set; }

    public string Sorting { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < SkipCount;

    public string MessageType { get; }

    public dynamic AggregateId { get; set; }

    public DateTime Timestamp { get; set; }

    public object Data { get; set; }

    public ValidationResult ValidationResult { get; set; }

    public async Task<bool> Validate(IBaseRequest request, IMediatorHandler handler)
    {
        if (request == null)
        {
            ValidationResult = await new TValidator().ValidateAsync(((TRequest)request)!);
        }

        if (ValidationResult is null)
        {
            throw new Exception("Erro ao efetuar validação!");
        }

        var isValid = ValidationResult.IsValid && ValidationResult.Errors?.Count == 0;

        if (!isValid)
        {
            foreach (var error in ValidationResult.Errors!)
            {
                await handler.PublishNotification(new DomainNotification(nameof(ValidationResult),
                    error.ErrorMessage));
            }
        }

        return isValid;
    }

    public object GetMetadataPagination()
    {
        return new
        {
            MaxResultCount,
            PageSize,
            CurrentPage,
            SkipCount,
            HasNext,
            HasPrevious
        };
    }
}

public abstract class QueryHandler<TRequest> : CommandHandler<TRequest>, IQueryHandler<TRequest>
    where TRequest : IRequest<Unit>
{
    protected QueryHandler(IMediatorHandler mediatorHandler) : base(mediatorHandler)
    {
    }

    public async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return await Execute(request, cancellationToken);
    }
}

public abstract class QueryHandler<TRequest, TResponse> : CommandHandler<TRequest, TResponse>,
    IQueryHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected QueryHandler(IMediatorHandler mediatorHandler) : base(mediatorHandler)
    {
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return await Execute(request, cancellationToken);
    }
}