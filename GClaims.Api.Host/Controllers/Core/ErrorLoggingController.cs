using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.DomainEvents;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.Core;
using log4net;
using log4net.Appender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayService.Api.Controllers.UniversalPay;

[Authorize(Policy = "MASTER")]
[Route("api/services/[controller]/[action]")]
public class ErrorLoggingController : AppControllerBase
{
    public ErrorLoggingController(INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediatorHandler) : base(notifications, mediatorHandler)
    {
      
    }

    [HttpGet]
    public IActionResult Index()
    {
        var path = (LogManager.GetCurrentLoggers()[0].Logger.Repository.GetAppenders()[0] as FileAppender).File;
        var result = new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
            "binary/octet-stream");
        result.FileDownloadName = "Logs.log";
        return result;
    }
}