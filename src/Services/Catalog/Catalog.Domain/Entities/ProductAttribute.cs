namespace Catalog.Domain.Entities;

public class ProductAttribute : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private ProductAttribute() { }

    public static ProductAttribute Create(Guid productId, string name, string value)
        => new()
        {
            ProductId = productId,
            Name = name,
            Value = value
        };
}
