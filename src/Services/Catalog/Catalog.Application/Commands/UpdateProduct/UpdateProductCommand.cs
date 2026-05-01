using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    decimal? CompareAtPrice,
    string SKU,
    Guid CategoryId,
    bool IsFeatured,
    List<CreateProductImageRequest>? Images,
    List<CreateProductAttributeRequest>? Attributes) : IRequest<Result<bool>>;
