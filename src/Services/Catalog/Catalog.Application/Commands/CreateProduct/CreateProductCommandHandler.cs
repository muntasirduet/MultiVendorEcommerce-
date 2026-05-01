using Catalog.Application.Common;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CompareAtPrice,
            request.SKU,
            request.VendorId,
            request.CategoryId,
            request.IsFeatured);

        if (request.Images is { Count: > 0 })
        {
            foreach (var img in request.Images)
                product.AddImage(ProductImage.Create(product.Id, img.Url, img.AltText, img.DisplayOrder, img.IsPrimary));
        }

        if (request.Attributes is { Count: > 0 })
        {
            foreach (var attr in request.Attributes)
                product.AddAttribute(ProductAttribute.Create(product.Id, attr.Name, attr.Value));
        }

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }
}
