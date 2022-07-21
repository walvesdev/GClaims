using FluentValidation;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.DomainEvents;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using MediatR;

namespace GClaims.BuildingBlocks.Core.Mediator;

public interface IMediatorHandler
{
    Task<bool> SendCommand<T>(T command) where T : ICommandMessage<bool>;

    Task<TResponse> SendCommand<TRequest, TResponse>(TRequest command)
        where TRequest : IRequest<TResponse>; //where TRequest : ICommandMessage<TResponse>;

    Task PublishEvent<T>(T domainEvent) where T : IEvent;

    Task PublishDomainEvent<T>(T notification) where T : IDomainEvent;

    Task PublishNotification<T>(T notification) where T : IDomainNotification;

    Task<TResponse> SendQuery<TRequest, TResponse>(TRequest command)
        where TRequest : IRequest<TResponse>, new();

    Task<TResponse> SendQuery<TRequest, TResponse, TValidator>(TRequest command)
        where TRequest : IBaseRequest, IRequest<TResponse>
        where TValidator : AbstractValidator<TRequest>, new();

    Task<bool> Validate<TRequest, TValidator>(TRequest command)
        where TRequest : IBaseRequest
        where TValidator : AbstractValidator<TRequest>, new();
}