using System.ComponentModel.DataAnnotations.Schema;
using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Core;
using GClaims.Core.Extensions;

namespace GClaims.BuildingBlocks.Core.Common;

public interface IAggregateRoot
{
    [NotMapped] IReadOnlyCollection<Event> Notifications { get; }

    void AddEnvent(Event domainEvent);
    void RemoveEvent(Event eventItem);
    void ClearEvents();
    bool IsValid();
}

public abstract class AggregateRoot<TKey> : EntityBaseAudit<TKey>, IAggregateRoot
    where TKey : struct
{
    [NotMapped] private List<Event> _notifications;

    [NotMapped] public IReadOnlyCollection<Event> Notifications => _notifications?.AsReadOnly()!;

    public void AddEnvent(Event domainEvent)
    {
        _notifications ??= new List<Event>();
        _notifications.Add(domainEvent);
    }

    public void RemoveEvent(Event eventItem)
    {
        _notifications?.Remove(eventItem);
    }

    public void ClearEvents()
    {
        _notifications?.Clear();
    }

    public virtual bool IsValid()
    {
        throw new NotImplementedException();
    }

    public void Associate<T>(T obj)
        where T : AggregateRoot<TKey>
    {
        obj.Id = Id;
    }
}

public static class AggregateRooExtensions
{
    public static async Task<bool> Update<T>(this T source, T target, OperationType operation = OperationType.Add)
        where T : class, new()
    {
        const bool success = false;

        if (operation is OperationType.Update or OperationType.Add)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
        }

        switch (operation)
        {
            case OperationType.Add:
            case OperationType.Update:
                Setvalue(source, target);
                break;
            case OperationType.Delete:
                break;
            case OperationType.Clear:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }

        return await Task.FromResult(success);
    }

    public static async Task<bool> Update<T>(this ICollection<T> aggregateCollection, T source,
        OperationType operation = OperationType.Add)
        where T : class, new()
    {
        const bool success = false;

        if (aggregateCollection == null || source == null)
        {
            return success;
        }

        var target = aggregateCollection.First(t => t.CompareId(source));

        if (operation is OperationType.Update or OperationType.Delete or OperationType.Clear)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
        }

        switch (operation)
        {
            case OperationType.Add:
                aggregateCollection.Add(source);
                break;
            case OperationType.Update:
                Setvalue(source, target);
                break;
            case OperationType.Delete:
                aggregateCollection.Remove(target!);
                break;
            case OperationType.Clear:
                aggregateCollection.Clear();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }

        return await Task.FromResult(success);
    }

    private static void Setvalue<T>(this T source, T target)
        where T : class, new()
    {
        if (source == null || target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        target.Assign(source);
    }
}