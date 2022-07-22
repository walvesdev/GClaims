using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace GClaims.Core.Extensions;

public static class ModelStateExtensions
{
    public static object ToErrorResponse(this ModelStateDictionary modelState,
        List<DomainNotification>? notifications = null, int code = 0, string? details = null, string? message = null)
    {
        var hasNotifcations = notifications?.Count > 0;

        if (modelState.IsValid && !hasNotifcations)
        {
            return new JsonResult(null);
        }

        var validationErrors = new List<dynamic>();

        foreach (var state in modelState)
        {
            foreach (var error in state.Value.Errors)
            {
                validationErrors.Add(new
                {
                    error.ErrorMessage,
                    state.Key
                });
            }
        }

        if (notifications == null)
        {
            return new JsonResult(new
            {
                Code = code,
                Details = details,
                Message = message ?? "Erro ao processar requisição!",
                ValidationErrors = validationErrors.ToArray()
            }, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        foreach (var notification in notifications)
        {
            
            validationErrors.Add(new
            {
                notification.Key,
                notification.Value,
                notification.Timestamp,
                notification.DomainNotificationId,
                notification.Version,
                notification.AggregateId,
                notification.MessageType,
                notification.Data,
            });
            
            if (notification?.ValidationResult != null && !notification.ValidationResult.Errors.IsNullOrEmpty())
            {
                foreach (var error in notification?.ValidationResult?.Errors)
                {
                    validationErrors.Add(new
                    {
                        error.ErrorCode,
                        error.ErrorMessage,
                        error.PropertyName,
                        notification.Key
                    });
                }
            }
        }

        return new JsonResult(new
        {
            Code = code,
            Details = details,
            Message = message ?? "Erro ao processar requisição!",
            ValidationErrors = validationErrors.ToList()
        });
    }
}