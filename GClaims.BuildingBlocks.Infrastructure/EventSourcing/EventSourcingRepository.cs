using System.Text;
using EventStore.ClientAPI;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GClaims.BuildingBlocks.Infrastructure.EventSourcing;

public class EventSourcingRepository : IEventSourcingRepository
{
    private readonly IEventStoreService _eventStoreService;

    public EventSourcingRepository(
        IEventStoreService eventStoreService,
        IConfiguration configuration,
        IOptions<AppSettingsSection> options)
    {
        _eventStoreService = eventStoreService;
        AppSettings = options.Value;
        UseEventStore = configuration.GetValue<bool>("AppSettings:EventStore:Enabled");

    }

    protected AppSettingsSection AppSettings { get; set; }
    public bool UseEventStore { get; set; }

    public async Task Save<TEvent>(TEvent integrationEvent) where TEvent : IEvent
    {
        if (!UseEventStore)
        {
            return;
        }

        await _eventStoreService.GetConnection().AppendToStreamAsync(
            integrationEvent?.AggregateId?.ToString(),
            ExpectedVersion.Any,
            FormatEvent(integrationEvent!));
    }

    public async Task<IEnumerable<StoredEvent>> GetEvents(Guid aggregateId)
    {
        if (!UseEventStore)
        {
            return new List<StoredEvent>();
        }

        var events = await _eventStoreService.GetConnection()
            .ReadStreamEventsForwardAsync(aggregateId.ToString(), 0, 500, false);

        var eventsList = new List<StoredEvent>();

        foreach (var resolvedEvent in events.Events)
        {
            var dataEncoded = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var jsonData = JsonConvert.DeserializeObject<BaseEvent>(dataEncoded);

            var domainEnvent = new StoredEvent(
                resolvedEvent.Event.EventId,
                resolvedEvent.Event.EventType,
                jsonData.Timestamp,
                dataEncoded);

            eventsList.Add(domainEnvent);
        }

        return eventsList.OrderBy(e => e.Date);
    }

    private static IEnumerable<EventData> FormatEvent<TEvent>(TEvent domainEnvent) where TEvent : IEvent
    {
        yield return new EventData(
            Guid.NewGuid(),
            domainEnvent.MessageType,
            true,
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEnvent)),
            null);
    }
}

internal class BaseEvent
{
    public DateTime Timestamp { get; set; }
}