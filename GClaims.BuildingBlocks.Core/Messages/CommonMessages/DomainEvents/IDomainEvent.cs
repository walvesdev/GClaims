using MediatR;

namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.DomainEvents;

public interface IDomainEvent : IMessage, INotification
{
    
}