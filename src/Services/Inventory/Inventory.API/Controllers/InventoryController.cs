using Inventory.Application.Commands.ConfirmReservation;
using Inventory.Application.Commands.ReleaseReservation;
using Inventory.Application.Commands.ReserveStock;
using Inventory.Application.Commands.UpdateStock;
using Inventory.Application.DTOs;
using Inventory.Application.Queries.GetInventoryByProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetInventoryByProductQuery(productId), ct);
        if (!result.IsSuccess)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("reserve")]
    [Authorize]
    public async Task<IActionResult> Reserve([FromBody] ReserveStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReserveStockCommand(request.OrderId, request.ProductId, request.Quantity, request.TtlMinutes), ct);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return Ok(new { reservationId = result.Value });
    }

    [HttpPost("release")]
    [Authorize]
    public async Task<IActionResult> Release([FromBody] ReleaseReservationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReleaseReservationCommand(request.OrderId, request.ProductId), ct);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return NoContent();
    }

    [HttpPost("confirm")]
    [Authorize]
    public async Task<IActionResult> Confirm([FromBody] ConfirmReservationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmReservationCommand(request.OrderId, request.ProductId), ct);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return NoContent();
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Roles = "vendor,admin")]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateStockCommand(productId, request.VendorId, request.Quantity, request.LowStockThreshold), ct);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return NoContent();
    }
}
