using System.ComponentModel.DataAnnotations;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.BuildingBlocks.Infrastructure.EventSourcing;
using GClaims.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wapps.Controllers;

[Authorize(Policy = "MASTER")]
[Route("api/services/app/[controller]/[action]")]
public class EventController : AppControllerBase
{
    private readonly IMediatorHandler _mediatorHandler;
    private readonly IEventSourcingRepository _eventSourcingRepository;

    public EventController(INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediatorHandler,
        IEventSourcingRepository eventSourcingRepository) : base(notifications, mediatorHandler)
    {
        _mediatorHandler = mediatorHandler;
        _eventSourcingRepository = eventSourcingRepository;
    }

    /// <summary>
    /// Obter todos os eventos de uma determiinada raiz de agregação
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{aggregateId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<StoredEvent>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetEvents([Required] Guid aggregateId)
    {
        var eventos = (await _eventSourcingRepository.GetEvents(aggregateId))?.ToList()!;

        if (eventos != null && eventos.Count > 0)
        {
            return Ok(eventos);
        }

        return NoContent();
    }
}