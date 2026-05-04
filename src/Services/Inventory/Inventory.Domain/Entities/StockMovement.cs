using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; private set; }
    public StockMovementType MovementType { get; private set; }
    public int Quantity { get; private set; }
    public int QuantityBefore { get; private set; }
    public int QuantityAfter { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(Guid productId, StockMovementType type, int quantity, int before, int after, string? reference = null, string? notes = null)
        => new() { ProductId = productId, MovementType = type, Quantity = quantity, QuantityBefore = before, QuantityAfter = after, Reference = reference, Notes = notes };
}
