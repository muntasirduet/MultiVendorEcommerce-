namespace Catalog.Application.DTOs;

public class CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentId { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class CategoryTreeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentId { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<CategoryTreeDto> Children { get; init; } = [];
}

public record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null,
    int DisplayOrder = 0);

public record UpdateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder);
