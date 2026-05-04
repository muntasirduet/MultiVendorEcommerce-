using Inventory.Application.Common;
using Inventory.Application.Interfaces;
using Inventory.Domain.Events;
using MassTransit;
using MediatR;

namespace Inventory.Application.Commands.ReleaseReservation;

public class ReleaseReservationCommandHandler : IRequestHandler<ReleaseReservationCommand, Result<bool>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IStockReservationRepository _reservationRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReleaseReservationCommandHandler(
        IInventoryRepository inventoryRepository,
        IStockReservationRepository reservationRepository,
        IPublishEndpoint publishEndpoint)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(ReleaseReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByOrderAndProductAsync(request.OrderId, request.ProductId, cancellationToken);
        if (reservation is null)
            return Result<bool>.Success(true); // idempotent

        var quantity = reservation.Quantity;
        reservation.Release();

        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (item is not null)
        {
            item.ReleaseReservation(quantity);
            await _inventoryRepository.UpdateAsync(item, cancellationToken);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new InventoryReleasedEvent
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            Quantity = quantity
        }, cancellationToken);

        return Result<bool>.Success(true);
    }
}
