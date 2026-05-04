namespace Inventory.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid VendorId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int LowStockThreshold { get; private set; } = 5;
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
    public bool IsLowStock => QuantityAvailable <= LowStockThreshold;

    private InventoryItem() { }

    public static InventoryItem Create(Guid productId, Guid vendorId, int initialQuantity, int lowStockThreshold = 5)
        => new() { ProductId = productId, VendorId = vendorId, QuantityOnHand = initialQuantity, LowStockThreshold = lowStockThreshold };

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        QuantityOnHand += quantity;
        SetUpdatedAt();
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (quantity > QuantityOnHand) throw new InvalidOperationException("Insufficient stock.");
        QuantityOnHand -= quantity;
        SetUpdatedAt();
    }

    public bool TryReserve(int quantity)
    {
        if (QuantityAvailable < quantity) return false;
        QuantityReserved += quantity;
        SetUpdatedAt();
        return true;
    }

    public void ReleaseReservation(int quantity)
    {
        QuantityReserved = Math.Max(0, QuantityReserved - quantity);
        SetUpdatedAt();
    }

    public void ConfirmReservation(int quantity)
    {
        QuantityReserved = Math.Max(0, QuantityReserved - quantity);
        QuantityOnHand = Math.Max(0, QuantityOnHand - quantity);
        SetUpdatedAt();
    }

    public void SetLowStockThreshold(int threshold) { LowStockThreshold = threshold; SetUpdatedAt(); }
}
