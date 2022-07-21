#region

using System.ComponentModel.DataAnnotations;
using GClaims.BuildingBlocks.Core;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using GClaims.Core;
using GClaims.Core.Auth;
using GClaims.Marvel.Application.Accounts.Comands;
using GClaims.Marvel.Application.Accounts.Queries;
using GClaims.Marvel.Application.Accounts.Requests;
using GClaims.Marvel.Application.Accounts.Responses;
using GClaims.Marvel.Application.Validators.AccountValidators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace GClaims.Host.Controllers.Modules.Marvel;

// [Authorize(Roles = AuthRoles.ALL)]
[Authorize(Policy = "MASTER")]
[Route("api/services/[controller]/[action]")]
public class AccountController : AppControllerBase
{
    public AccountController(INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediatorHandler) : base(notifications, mediatorHandler)
    {
    }

    /// <summary>
    /// Insere um(a) Account
    /// </summary>
    /// <param name="request">request</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] [Required] CreateAccountRequest request)
    {
        var result =
            await MediatorHandler.SendCommand<CreateAccountCommand, CreateAccountResponse>(
                new CreateAccountCommand(request));

        if (!OperationIsValid)
        {
            return Errors();
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza um(a) Account
    /// </summary>
    /// <param name="request">request</param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(BaseAccountResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] [Required] UpdateAccountRequest request)
    {
        await MediatorHandler.SendCommand<UpdateAccountCommand, bool>(new UpdateAccountCommand(request));

        if (!OperationIsValid)
        {
            return Errors();
        }

        return Ok();
    }

    /// <summary>
    /// Deleta um(a) Account
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([Required] [FromRoute] [Range(1, int.MaxValue)] int id)
    {
        await MediatorHandler.SendCommand<DeleteAccountCommand, bool>(new DeleteAccountCommand(id));

        if (!OperationIsValid)
        {
            return Errors();
        }

        return NoContent();
    }

    /// <summary>
    /// Retorna uma lista de Account
    /// </summary>
    /// <param name="request">request</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetAllAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromBody] [Required] GetAllAccountRequest request)
    {
        var response =
            await MediatorHandler.SendQuery<GetAllAccountQuery, GetAllAccountResponse, GetAllAccountValidator>(
                new GetAllAccountQuery(request));

        if (!OperationIsValid)
        {
            return Errors();
        }

        return Ok(response);
    }
}