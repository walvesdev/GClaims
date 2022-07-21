using MediatR;

namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;

public interface IDomainNotification : IMessage, INotification
{
    
}