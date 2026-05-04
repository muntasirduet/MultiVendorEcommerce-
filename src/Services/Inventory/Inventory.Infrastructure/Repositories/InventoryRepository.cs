using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context) => _context = context;

    public Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
        => _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, ct);

    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
        => await _context.InventoryItems.AddAsync(item, ct);

    public Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        _context.InventoryItems.Update(item);
        return Task.CompletedTask;
    }

    public async Task AddStockMovementAsync(StockMovement movement, CancellationToken ct = default)
        => await _context.StockMovements.AddAsync(movement, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
