using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType<List<UserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<UserDto>>> Search(
        [FromQuery] string q,
        CancellationToken ct)
    {
        var result = await usersService.Search(q, ct);
        if (result.IsFailed)
            return BadRequest(result.Errors[0].Message);

        return Ok(result.Value);
    }
}