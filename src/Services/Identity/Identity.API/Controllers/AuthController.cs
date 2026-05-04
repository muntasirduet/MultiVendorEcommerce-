using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.RefreshToken;
using Identity.Application.Commands.Register;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(
            request.Email, request.Password, request.FirstName, request.LastName, request.PhoneNumber, request.Role);

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return result.Error!.Code switch
            {
                "CONFLICT" => Conflict(new { result.Error.Message }),
                "EXTERNAL_SERVICE_ERROR" => StatusCode(StatusCodes.Status502BadGateway, new { result.Error.Message }),
                _ => BadRequest(new { result.Error.Message })
            };
        }

        return CreatedAtAction(nameof(Register), new { userId = result.Value });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);

        if (!result.IsSuccess)
        {
            return result.Error!.Code switch
            {
                "UNAUTHORIZED" => Unauthorized(new { result.Error.Message }),
                _ => BadRequest(new { result.Error.Message })
            };
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        if (!result.IsSuccess)
        {
            return result.Error!.Code switch
            {
                "UNAUTHORIZED" => Unauthorized(new { result.Error.Message }),
                _ => BadRequest(new { result.Error.Message })
            };
        }

        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LogoutCommand(request.RefreshToken), ct);

        if (!result.IsSuccess)
            return BadRequest(new { result.Error!.Message });

        return NoContent();
    }
}
