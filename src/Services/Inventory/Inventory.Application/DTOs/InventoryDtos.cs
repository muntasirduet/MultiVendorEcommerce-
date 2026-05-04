namespace Inventory.Application.DTOs;

public class InventoryItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public Guid VendorId { get; init; }
    public int QuantityOnHand { get; init; }
    public int QuantityReserved { get; init; }
    public int QuantityAvailable { get; init; }
    public int LowStockThreshold { get; init; }
    public bool IsLowStock { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record UpdateStockRequest(Guid ProductId, Guid VendorId, int Quantity, int LowStockThreshold = 5);
public record ReserveStockRequest(Guid OrderId, Guid ProductId, int Quantity, int TtlMinutes = 15);
public record ReleaseReservationRequest(Guid OrderId, Guid ProductId);
public record ConfirmReservationRequest(Guid OrderId, Guid ProductId);
