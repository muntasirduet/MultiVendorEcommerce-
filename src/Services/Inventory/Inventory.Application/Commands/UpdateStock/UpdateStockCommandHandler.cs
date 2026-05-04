using Inventory.Application.Common;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using MassTransit;
using MediatR;

namespace Inventory.Application.Commands.UpdateStock;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result<bool>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateStockCommandHandler(IInventoryRepository inventoryRepository, IPublishEndpoint publishEndpoint)
    {
        _inventoryRepository = inventoryRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        int quantityBefore;
        if (item is null)
        {
            quantityBefore = 0;
            item = InventoryItem.Create(request.ProductId, request.VendorId, request.Quantity, request.LowStockThreshold);
            await _inventoryRepository.AddAsync(item, cancellationToken);
        }
        else
        {
            quantityBefore = item.QuantityOnHand;
            var diff = request.Quantity - item.QuantityOnHand;
            if (diff > 0)
                item.AddStock(diff);
            else if (diff < 0)
                item.ReduceStock(-diff);

            item.SetLowStockThreshold(request.LowStockThreshold);
            await _inventoryRepository.UpdateAsync(item, cancellationToken);
        }

        var movementQty = Math.Abs(item.QuantityOnHand - quantityBefore);
        var movement = StockMovement.Create(
            item.ProductId,
            StockMovementType.Adjustment,
            movementQty,
            quantityBefore,
            item.QuantityOnHand);

        await _inventoryRepository.AddStockMovementAsync(movement, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new InventoryUpdatedEvent
        {
            ProductId = item.ProductId,
            VendorId = item.VendorId,
            QuantityOnHand = item.QuantityOnHand,
            QuantityAvailable = item.QuantityAvailable,
            IsLowStock = item.IsLowStock
        }, cancellationToken);

        if (item.IsLowStock)
        {
            await _publishEndpoint.Publish(new LowStockAlertEvent
            {
                ProductId = item.ProductId,
                VendorId = item.VendorId,
                QuantityAvailable = item.QuantityAvailable,
                LowStockThreshold = item.LowStockThreshold
            }, cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}
