using Catalog.Application.Commands.CreateProduct;
using Catalog.Application.Commands.DeleteProduct;
using Catalog.Application.Commands.UpdateProduct;
using Catalog.Application.DTOs;
using Catalog.Application.Queries.GetProductById;
using Catalog.Application.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Gets a paginated list of active products with optional filtering.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Application.Common.PaginatedResult<ProductSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? vendorId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(categoryId, vendorId, minPrice, maxPrice, search, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>Gets a single product by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>Creates a new product. Requires vendor or admin role.</summary>
    [HttpPost]
    [Authorize(Roles = "vendor,admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.CompareAtPrice,
            request.SKU,
            request.VendorId,
            request.CategoryId,
            request.IsFeatured,
            request.Images?.Select(i => new CreateProductImageRequest(i.Url, i.AltText, i.DisplayOrder, i.IsPrimary)).ToList(),
            request.Attributes?.Select(a => new CreateProductAttributeRequest(a.Name, a.Value)).ToList());

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, result.Value);
    }

    /// <summary>Updates an existing product. Requires vendor or admin role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "vendor,admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.CompareAtPrice,
            request.SKU,
            request.CategoryId,
            request.IsFeatured,
            request.Images?.Select(i => new CreateProductImageRequest(i.Url, i.AltText, i.DisplayOrder, i.IsPrimary)).ToList(),
            request.Attributes?.Select(a => new CreateProductAttributeRequest(a.Name, a.Value)).ToList());

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Code == "NOT_FOUND" ? NotFound(result.Error) : BadRequest(result.Error);

        return NoContent();
    }

    /// <summary>Soft-deletes (deactivates) a product. Requires vendor or admin role.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "vendor,admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return NoContent();
    }
}
