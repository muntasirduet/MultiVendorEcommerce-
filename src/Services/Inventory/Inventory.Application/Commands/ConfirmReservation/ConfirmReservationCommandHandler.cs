using Inventory.Application.Common;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using MassTransit;
using MediatR;

namespace Inventory.Application.Commands.ConfirmReservation;

public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, Result<bool>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IStockReservationRepository _reservationRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ConfirmReservationCommandHandler(
        IInventoryRepository inventoryRepository,
        IStockReservationRepository reservationRepository,
        IPublishEndpoint publishEndpoint)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByOrderAndProductAsync(request.OrderId, request.ProductId, cancellationToken);
        if (reservation is null)
            return Result<bool>.Failure(Error.NotFound("StockReservation"));

        var quantity = reservation.Quantity;
        reservation.Confirm();

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (item is null)
            return Result<bool>.Failure(Error.NotFound("InventoryItem"));

        var quantityBefore = item.QuantityOnHand;
        item.ConfirmReservation(quantity);

        var movement = StockMovement.Create(
            item.ProductId,
            StockMovementType.Sale,
            quantity,
            quantityBefore,
            item.QuantityOnHand);

        await _inventoryRepository.AddStockMovementAsync(movement, cancellationToken);
        await _inventoryRepository.UpdateAsync(item, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new InventoryUpdatedEvent
        {
            ProductId = item.ProductId,
            VendorId = item.VendorId,
            QuantityOnHand = item.QuantityOnHand,
            QuantityAvailable = item.QuantityAvailable,
            IsLowStock = item.IsLowStock
        }, cancellationToken);

        return Result<bool>.Success(true);
    }
}
