using EventStore.ClientAPI;

namespace GClaims.BuildingBlocks.Infrastructure.EventSourcing;

public interface IEventStoreService
{
    IEventStoreConnection GetConnection();
}