using Inventory.Application.Common;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Events;
using MassTransit;
using MediatR;

namespace Inventory.Application.Commands.ReserveStock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, Result<Guid>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IStockReservationRepository _reservationRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReserveStockCommandHandler(
        IInventoryRepository inventoryRepository,
        IStockReservationRepository reservationRepository,
        IPublishEndpoint publishEndpoint)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (item is null)
            return Result<Guid>.Failure(Error.NotFound("InventoryItem"));

        var reserved = item.TryReserve(request.Quantity);
        if (!reserved)
            return Result<Guid>.Failure(Error.Conflict("Insufficient stock available for reservation."));

        var reservation = StockReservation.Create(request.OrderId, request.ProductId, request.Quantity, request.TtlMinutes);
        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _inventoryRepository.UpdateAsync(item, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new InventoryReservedEvent
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            ExpiresAt = reservation.ExpiresAt
        }, cancellationToken);

        return Result<Guid>.Success(reservation.Id);
    }
}
