using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            _notifications.Add(message);
            return Task.CompletedTask;
        }

        public virtual List<DomainNotification> GetNotifications()
        {
            return _notifications;
        }

        protected virtual bool HasNotification()
        {
            return GetNotifications().Any();
        }
        
        public virtual bool HasNotification(ModelStateDictionary modelState = null)
        {
            var hasNotification = HasNotification();

            if (hasNotification && modelState is not null)
            {
                _notifications.ForEach(c => modelState.AddModelError(string.Empty, c.Value));
            }
            
            return hasNotification;
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
        }
    }
}