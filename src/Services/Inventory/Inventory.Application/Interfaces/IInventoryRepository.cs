using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(InventoryItem item, CancellationToken ct = default);
    Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
    Task AddStockMovementAsync(StockMovement movement, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
