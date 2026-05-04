using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IStockReservationRepository
{
    Task<StockReservation?> GetByOrderAndProductAsync(Guid orderId, Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<StockReservation>> GetExpiredReservationsAsync(CancellationToken ct = default);
    Task AddAsync(StockReservation reservation, CancellationToken ct = default);
    Task UpdateAsync(StockReservation reservation, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
