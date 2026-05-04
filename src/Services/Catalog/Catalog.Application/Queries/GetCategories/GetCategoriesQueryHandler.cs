using Catalog.Application.Common;
using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryTreeDto>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<Result<IReadOnlyList<CategoryTreeDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllWithChildrenAsync(cancellationToken);

        var rootCategories = categories
            .Where(c => c.ParentId is null)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => MapToTreeDto(c, categories))
            .ToList();

        return Result<IReadOnlyList<CategoryTreeDto>>.Success(rootCategories);
    }

    private static CategoryTreeDto MapToTreeDto(Category category, IReadOnlyList<Category> allCategories)
    {
        var children = allCategories
            .Where(c => c.ParentId == category.Id)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => MapToTreeDto(c, allCategories))
            .ToList();

        return new CategoryTreeDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            Children = children
        };
    }
}
