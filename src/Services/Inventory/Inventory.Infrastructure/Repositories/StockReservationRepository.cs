using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class StockReservationRepository : IStockReservationRepository
{
    private readonly InventoryDbContext _context;

    public StockReservationRepository(InventoryDbContext context) => _context = context;

    public Task<StockReservation?> GetByOrderAndProductAsync(Guid orderId, Guid productId, CancellationToken ct = default)
        => _context.StockReservations
            .FirstOrDefaultAsync(r => r.OrderId == orderId && r.ProductId == productId, ct);

    public async Task<IReadOnlyList<StockReservation>> GetExpiredReservationsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.StockReservations
            .Where(r => r.Status == ReservationStatus.Pending && r.ExpiresAt < now)
            .ToListAsync(ct);
    }

    public async Task AddAsync(StockReservation reservation, CancellationToken ct = default)
        => await _context.StockReservations.AddAsync(reservation, ct);

    public Task UpdateAsync(StockReservation reservation, CancellationToken ct = default)
    {
        _context.StockReservations.Update(reservation);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
