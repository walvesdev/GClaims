namespace GClaims.BuildingBlocks.Infrastructure.EventSourcing;

public class StoredEvent
{
    public StoredEvent(Guid id, string type, DateTime date, string data)
    {
        Id = id;
        Type = type;
        Date = date;
        Data = data;
    }

    public Guid Id { get; }

    public string Type { get; }

    public DateTime Date { get; set; }

    public string Data { get; }
}