using Catalog.Application.Common;
using Catalog.Application.Interfaces;
using MediatR;

namespace Catalog.Application.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result<bool>.Failure(Error.NotFound("Product"));

        product.Deactivate();

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
