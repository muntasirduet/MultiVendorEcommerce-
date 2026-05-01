using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    decimal? CompareAtPrice,
    string SKU,
    Guid VendorId,
    Guid CategoryId,
    bool IsFeatured,
    List<CreateProductImageRequest>? Images,
    List<CreateProductAttributeRequest>? Attributes) : IRequest<Result<Guid>>;
