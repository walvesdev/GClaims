using FluentValidation;
using GClaims.BuildingBlocks.Core.Mediator;
using MediatR;

namespace GClaims.BuildingBlocks.Core.Messages;

public interface ICommunication : IMessage
{
}

public interface ICommandMessage : ICommunication, IRequest
{
}

public interface ICommandMessage<TResponse> : ICommunication, IRequest<TResponse>
{
}

public interface ICommandMessage<TRequest, TResponse> : ICommunication, IRequest<TResponse>
{
}

public interface ICommandMessage<TRequest, TResponse, TValidator, TInput> : ICommandMessage<TRequest, TResponse>
    where TValidator : AbstractValidator<TRequest>
    where TInput : class, new()
    where TRequest : IRequest<TResponse>
{
}

public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest<Unit>
{
    public IMediatorHandler MediatorHandler { get; set; }
}

public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public IMediatorHandler MediatorHandler { get; set; }
}

public interface IQueryMessage<TRequest, TResponse> : ICommandMessage<TRequest, TResponse>, ICommandQuery
    where TRequest : IRequest<TResponse>
{
}

public interface
    IQueryMessage<TRequest, TResponse, TValidator, TInput> : ICommandMessage<TRequest, TResponse, TValidator, TInput>,
        ICommandQuery
    where TRequest : IBaseRequest, IRequest<TResponse>
    where TInput : class, new()
    where TValidator : AbstractValidator<TRequest>, new()
{
}

public interface IQueryHandler<in TRequest> : ICommandHandler<TRequest>
    where TRequest : IRequest<Unit>
{
    public IMediatorHandler MediatorHandler { get; set; }
}

public interface IQueryHandler<TRequest, TResponse> : ICommandHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public IMediatorHandler MediatorHandler { get; set; }
}