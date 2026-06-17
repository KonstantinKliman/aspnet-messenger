using System.Security.Claims;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Conversations;
using Messenger.Contracts.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize]
public class ConversationsController(IConversationsService conversationsService) : ControllerBase
{
    [HttpPost("direct")]
    [ProducesResponseType<ConversationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversationDto>> OpenDirect(
        [FromBody] OpenDirectRequest request,
        CancellationToken ct)
    {
        var result = await conversationsService.OpenDirectAsync(request.UserId, ct);
        
        if (result.IsFailed)
            return BadRequest(result.Errors[0].Message);

        return Ok(result.Value);

    }

    [HttpGet]
    [ProducesResponseType<List<ConversationListItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConversationListItemDto>>> List(CancellationToken ct)
    {
        var result = await conversationsService.ListAsync(ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ConversationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await conversationsService.GetByIdAsync(id, ct);
        
        if (result.IsFailed)
            return NotFound(result.Errors[0].Message);

        return Ok(result.Value);
    }
}