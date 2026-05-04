namespace Inventory.Domain.Events;

public record LowStockAlertEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = nameof(LowStockAlertEvent);
    public Guid ProductId { get; init; }
    public Guid VendorId { get; init; }
    public int QuantityAvailable { get; init; }
    public int LowStockThreshold { get; init; }
}
