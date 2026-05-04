using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetProducts;

public record GetProductsQuery(
    Guid? CategoryId = null,
    Guid? VendorId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<ProductSummaryDto>>>;
