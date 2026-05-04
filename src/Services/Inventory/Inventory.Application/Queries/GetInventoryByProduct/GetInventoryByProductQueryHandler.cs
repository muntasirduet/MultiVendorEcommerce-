using Inventory.Application.Common;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using MediatR;

namespace Inventory.Application.Queries.GetInventoryByProduct;

public class GetInventoryByProductQueryHandler : IRequestHandler<GetInventoryByProductQuery, Result<InventoryItemDto>>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetInventoryByProductQueryHandler(IInventoryRepository inventoryRepository)
        => _inventoryRepository = inventoryRepository;

    public async Task<Result<InventoryItemDto>> Handle(GetInventoryByProductQuery request, CancellationToken cancellationToken)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (item is null)
            return Result<InventoryItemDto>.Failure(Error.NotFound("InventoryItem"));

        var dto = new InventoryItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            VendorId = item.VendorId,
            QuantityOnHand = item.QuantityOnHand,
            QuantityReserved = item.QuantityReserved,
            QuantityAvailable = item.QuantityAvailable,
            LowStockThreshold = item.LowStockThreshold,
            IsLowStock = item.IsLowStock,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };

        return Result<InventoryItemDto>.Success(dto);
    }
}
