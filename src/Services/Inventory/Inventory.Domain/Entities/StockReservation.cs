using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class StockReservation : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;
    public DateTime ExpiresAt { get; private set; }

    private StockReservation() { }

    public static StockReservation Create(Guid orderId, Guid productId, int quantity, int ttlMinutes = 15)
        => new() { OrderId = orderId, ProductId = productId, Quantity = quantity, Status = ReservationStatus.Pending, ExpiresAt = DateTime.UtcNow.AddMinutes(ttlMinutes) };

    public void Confirm() { Status = ReservationStatus.Confirmed; SetUpdatedAt(); }
    public void Release() { Status = ReservationStatus.Released; SetUpdatedAt(); }
    public void MarkExpired() { Status = ReservationStatus.Expired; SetUpdatedAt(); }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt && Status == ReservationStatus.Pending;
}
