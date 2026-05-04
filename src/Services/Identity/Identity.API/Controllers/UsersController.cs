using Identity.Application.Commands.AssignRoles;
using Identity.Application.DTOs;
using Identity.Application.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(id), ct);

        if (!result.IsSuccess)
            return NotFound(new { result.Error!.Message });

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/roles")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignRolesCommand(id, request.Roles), ct);

        if (!result.IsSuccess)
        {
            return result.Error!.Code switch
            {
                "NOT_FOUND" => NotFound(new { result.Error.Message }),
                _ => BadRequest(new { result.Error.Message })
            };
        }

        return NoContent();
    }
}
