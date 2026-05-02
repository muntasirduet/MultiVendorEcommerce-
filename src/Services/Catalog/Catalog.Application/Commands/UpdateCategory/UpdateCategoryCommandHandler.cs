using Catalog.Application.Common;
using Catalog.Application.Interfaces;
using MediatR;

namespace Catalog.Application.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
            return Result<bool>.Failure(Error.NotFound("Category"));

        category.Update(request.Name, request.Slug, request.Description, request.DisplayOrder);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
