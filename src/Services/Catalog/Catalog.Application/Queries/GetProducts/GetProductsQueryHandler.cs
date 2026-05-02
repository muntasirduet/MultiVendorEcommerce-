using Catalog.Application.Common;
using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;
using MediatR;

namespace Catalog.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductSummaryDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Result<PaginatedResult<ProductSummaryDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(
            request.CategoryId,
            request.VendorId,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            SKU = p.SKU,
            VendorId = p.VendorId,
            CategoryId = p.CategoryId,
            StockStatus = p.StockStatus,
            IsFeatured = p.IsFeatured,
            PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary)?.Url
                              ?? p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url
        }).ToList();

        var result = new PaginatedResult<ProductSummaryDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PaginatedResult<ProductSummaryDto>>.Success(result);
    }
}
