using Catalog.Domain.Enums;
using Catalog.Domain.Events;

namespace Catalog.Domain.Entities;

public class Product : BaseEntity
{
    private readonly List<ProductImage> _images = new();
    private readonly List<ProductAttribute> _attributes = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public decimal? CompareAtPrice { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public Guid VendorId { get; private set; }
    public Guid CategoryId { get; private set; }
    public StockStatus StockStatus { get; private set; } = StockStatus.InStock;
    public bool IsActive { get; private set; } = true;
    public bool IsFeatured { get; private set; }

    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyList<ProductAttribute> Attributes => _attributes.AsReadOnly();

    private Product() { }

    public static Product Create(
        string name,
        string? description,
        decimal price,
        decimal? compareAtPrice,
        string sku,
        Guid vendorId,
        Guid categoryId,
        bool isFeatured = false)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            CompareAtPrice = compareAtPrice,
            SKU = sku,
            VendorId = vendorId,
            CategoryId = categoryId,
            IsFeatured = isFeatured,
            StockStatus = StockStatus.InStock,
            IsActive = true
        };

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.VendorId, product.Price, product.CategoryId));
        return product;
    }

    public void UpdateDetails(
        string name,
        string? description,
        string sku,
        Guid categoryId,
        bool isFeatured)
    {
        Name = name;
        Description = description;
        SKU = sku;
        CategoryId = categoryId;
        IsFeatured = isFeatured;
        SetUpdatedAt();
        AddDomainEvent(new ProductUpdatedEvent(Id, Name, VendorId));
    }

    public void UpdatePrice(decimal newPrice, decimal? compareAtPrice)
    {
        if (newPrice != Price)
        {
            AddDomainEvent(new ProductPriceChangedEvent(Id, Price, newPrice));
            Price = newPrice;
        }
        CompareAtPrice = compareAtPrice;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        SetUpdatedAt();
        AddDomainEvent(new ProductDeactivatedEvent(Id, VendorId));
    }

    public void UpdateStockStatus(StockStatus status)
    {
        StockStatus = status;
        SetUpdatedAt();
    }

    public void AddImage(ProductImage image) => _images.Add(image);
    public void AddAttribute(ProductAttribute attribute) => _attributes.Add(attribute);

    public void ReplaceImages(IEnumerable<ProductImage> images)
    {
        _images.Clear();
        _images.AddRange(images);
    }

    public void ReplaceAttributes(IEnumerable<ProductAttribute> attributes)
    {
        _attributes.Clear();
        _attributes.AddRange(attributes);
    }
}
