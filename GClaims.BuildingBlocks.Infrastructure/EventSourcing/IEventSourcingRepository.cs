using GClaims.BuildingBlocks.Core.Messages;

namespace GClaims.BuildingBlocks.Infrastructure.EventSourcing;

public interface IEventSourcingRepository
{
    Task Save<TEvent>(TEvent domainEvent) where TEvent : IEvent;
    Task<IEnumerable<StoredEvent>> GetEvents(Guid aggregateId);
}