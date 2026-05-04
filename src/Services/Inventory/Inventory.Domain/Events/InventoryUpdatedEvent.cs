namespace Inventory.Domain.Events;

public record InventoryUpdatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = nameof(InventoryUpdatedEvent);
    public Guid ProductId { get; init; }
    public Guid VendorId { get; init; }
    public int QuantityOnHand { get; init; }
    public int QuantityAvailable { get; init; }
    public bool IsLowStock { get; init; }
}
