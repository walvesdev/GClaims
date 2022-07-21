using FluentValidation;
using FluentValidation.Results;
using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.DomainEvents;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.BuildingBlocks.Infrastructure.EventSourcing;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace GClaims.BuildingBlocks.Application.Mediator;

public class MediatorHandler : IMediatorHandler
{
    private readonly IEventSourcingRepository _eventSourcingRepository;
    private readonly IMediator _mediator;

    public MediatorHandler(IMediator mediator,
        IEventSourcingRepository eventSourcingRepository,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _eventSourcingRepository = eventSourcingRepository;
        UseEventStore = configuration.GetValue<bool>("AppSettings:EventStore:Enabled");
    }

    public bool UseEventStore { get; set; }

    public async Task<bool> SendCommand<T>(T command) where T : ICommandMessage<bool>
    {
        return await _mediator.Send(command);
    }

    public async Task<TResponse> SendCommand<TRequest, TResponse>(TRequest command)
        where TRequest : IRequest<TResponse>

    {
        return await _mediator.Send(command);
    }

    public async Task PublishEvent<T>(T domainEvent) where T : IEvent
    {
        await _mediator.Publish(domainEvent);

        if (UseEventStore && domainEvent?.AggregateId != null)
        {
            await _eventSourcingRepository.Save(domainEvent!);
        }
    }

    public async Task PublishNotification<T>(T notification) where T : IDomainNotification
    {
        await _mediator.Publish(notification);
    }

    public async Task PublishDomainEvent<T>(T notification) where T : IDomainEvent
    {
        await _mediator.Publish(notification);
    }

    public async Task<TResponse> SendQuery<TRequest, TResponse>(TRequest command)
        where TRequest : IRequest<TResponse>, new()
    {
        return await _mediator.Send(command);
    }

    public async Task<TResponse> SendQuery<TRequest, TResponse, TValidator>(TRequest command)
        where TRequest : IBaseRequest, IRequest<TResponse>
        where TValidator : AbstractValidator<TRequest>, new()
    {
        return await _mediator.Send(command);
    }

    public async Task<bool> Validate<TRequest, TValidator>(TRequest command)
        where TRequest : IBaseRequest
        where TValidator : AbstractValidator<TRequest>, new()
    {
        ValidationResult validationResult = new ValidationResult();
        var validator = new TValidator();

        if (command != null)
        {
            validationResult = validator.Validate(command!);
        }

        if (validationResult is null)
        {
            throw new Exception("Erro ao efetuar validação!");
        }

        var isValid = validationResult.IsValid && validationResult.Errors?.Count == 0;

        if (!isValid)
        {
            foreach (var error in validationResult.Errors!)
            {
                await PublishNotification(new DomainNotification(nameof(ValidationResult),
                    error.ErrorMessage));
            }
        }

        return isValid;
    }
}