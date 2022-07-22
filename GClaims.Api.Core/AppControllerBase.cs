using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GClaims.Core;

public abstract class AppControllerBase : Controller
{
    private readonly DomainNotificationHandler _notifications;
    private readonly IMediatorHandler _mediatorHandler;

    protected AppControllerBase(INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediatorHandler)
    {
        _notifications = (DomainNotificationHandler)notifications;
        _mediatorHandler = mediatorHandler;
    }
    
    protected bool OperationIsValid => !_notifications.HasNotification();

    protected bool ValidateOperation(ModelStateDictionary modelState)
    {
        return !_notifications.HasNotification(modelState);
    }

    protected IEnumerable<string> GetErrorMessages()
    {
        return _notifications.GetNotifications().Select(c => c.Value).ToList();
    }

    protected List<DomainNotification> GetNotifications()
    {
        return _notifications.GetNotifications();
    }

    protected void SendError(string code, string message)
    {
        _mediatorHandler.PublishNotification(new DomainNotification(code, message));
    }

    protected UnprocessableEntityObjectResult Errors()
    {
        return UnprocessableEntity(ModelState.ToErrorResponse(GetNotifications()));
    }

    protected UnprocessableEntityObjectResult Error(string message)
    {
        var validationErrors = new List<dynamic>
        {
            new
            {
                message, type = "ValidationError"
            }
        };

        return UnprocessableEntity(new
        {
            Code = 422,
            Details = "Hove erros de validação!",
            Message = "Erro ao processar requisição!",
            ValidationErrors = validationErrors.ToArray()
        });
    }
}