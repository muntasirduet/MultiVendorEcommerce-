using Catalog.Application.Common;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result<bool>.Failure(Error.NotFound("Product"));

        product.UpdateDetails(request.Name, request.Description, request.SKU, request.CategoryId, request.IsFeatured);
        product.UpdatePrice(request.Price, request.CompareAtPrice);

        if (request.Images is not null)
        {
            var images = request.Images.Select(img =>
                ProductImage.Create(product.Id, img.Url, img.AltText, img.DisplayOrder, img.IsPrimary));
            product.ReplaceImages(images);
        }

        if (request.Attributes is not null)
        {
            var attributes = request.Attributes.Select(attr =>
                ProductAttribute.Create(product.Id, attr.Name, attr.Value));
            product.ReplaceAttributes(attributes);
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
