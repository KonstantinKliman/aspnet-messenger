using Messenger.Api.Auth;
using Messenger.Api.BusinessLogic.Abstractions;
using Messenger.Api.Hubs;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Hubs.Interfaces;
using Messenger.Contracts.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Api.Controllers;

[ApiController]
[Route("api/conversations/{conversationId:guid}/messages")]
[Authorize]
public class MessagesController(IMessageService messageService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<MessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MessageDto>>> GetHistory(
        Guid conversationId,
        [FromQuery] Guid? before,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var result = await messageService.GetHistoryAsync(conversationId, limit, before, ct);
        if (result.IsFailed)
            return NotFound(result.Errors[0].Message);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType<MessageDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> Send(
        Guid conversationId,
        [FromBody] SendMessageRequest request,
        [FromServices] IHubContext<ChatHub, IChatClient> hub,
        CancellationToken ct)
    {
        var result = await messageService.SendAsync(conversationId, currentUser.UserId, request.Content, ct);

        if (result.IsFailed) 
            return NotFound();

        foreach (var userId in result.Value.Recipients)
            await hub.Clients.User(userId.ToString()).MessageReceived(result.Value.Message!);

        return CreatedAtAction(nameof(GetHistory), new { conversationId }, result.Value.Message);
    }

    [HttpPost("read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(
        Guid conversationId,
        [FromBody] MarkReadRequest request,
        CancellationToken ct)
    {
        var result = await messageService.MarkReadAsync(conversationId, request.MessageId, ct);

        return result.IsFailed 
            ? NotFound() 
            : NoContent();
    }
}