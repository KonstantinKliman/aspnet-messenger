using Mapster;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos;
using Messenger.Contracts.Requests.Auth;
using Messenger.Contracts.Responses.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request.Adapt<RegisterUserDto>(), ct);
        if (result.IsFailed)
            return BadRequest(result.Errors[0].Message);
        
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.Adapt<LoginUserDto>(), ct);
        if (result.IsFailed)
            return Unauthorized(result.Errors[0].Message);
        
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("refresh")]
    [ProducesResponseType<RefreshResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken ct)
    {
        var result = await authService.RefreshAsync(request.RefreshToken, ct);
        
        if (result.IsFailed)
            return BadRequest(result.Errors[0].Message);
        
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshRequest request,
        CancellationToken ct)
    {
        var result = await authService.LogoutAsync(request.RefreshToken, ct);
        
        if (result.IsFailed)
            return BadRequest(result.Errors[0].Message);
        
        return NoContent();
    }
}