using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages;
using MediatR;

namespace GClaims.BuildingBlocks.Infrastructure.EventSourcing;

public class EventTest : Event
{
    public EventTest(Guid id, string data)
    {
        Id = id;
        AggregateId = id;
        Data = data;
    }

    public Guid Id { get; }

    public new string Data { get; set; }
}

public class EventTestHandler : INotificationHandler<EventTest>
{
    private readonly IMediatorHandler _mediatorHandler;

    public EventTestHandler(IMediatorHandler mediatorHandler)
    {
        _mediatorHandler = mediatorHandler;
    }

    public async Task Handle(EventTest notification, CancellationToken cancellationToken)
    {
        await _mediatorHandler.PublishEvent(notification);
        Console.WriteLine(notification.ToString());
    }
}