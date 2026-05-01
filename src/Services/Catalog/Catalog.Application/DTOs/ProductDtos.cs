using Catalog.Domain.Enums;

namespace Catalog.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public string SKU { get; init; } = string.Empty;
    public Guid VendorId { get; init; }
    public Guid CategoryId { get; init; }
    public StockStatus StockStatus { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<ProductImageDto> Images { get; init; } = [];
    public IReadOnlyList<ProductAttributeDto> Attributes { get; init; } = [];
}

public class ProductSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public string SKU { get; init; } = string.Empty;
    public Guid VendorId { get; init; }
    public Guid CategoryId { get; init; }
    public StockStatus StockStatus { get; init; }
    public bool IsFeatured { get; init; }
    public string? PrimaryImageUrl { get; init; }
}

public class ProductImageDto
{
    public Guid Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string AltText { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
}

public class ProductAttributeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    decimal? CompareAtPrice,
    string SKU,
    Guid VendorId,
    Guid CategoryId,
    bool IsFeatured = false,
    List<CreateProductImageRequest>? Images = null,
    List<CreateProductAttributeRequest>? Attributes = null);

public record CreateProductImageRequest(string Url, string AltText, int DisplayOrder, bool IsPrimary = false);

public record CreateProductAttributeRequest(string Name, string Value);

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    decimal? CompareAtPrice,
    string SKU,
    Guid CategoryId,
    bool IsFeatured = false,
    List<CreateProductImageRequest>? Images = null,
    List<CreateProductAttributeRequest>? Attributes = null);
