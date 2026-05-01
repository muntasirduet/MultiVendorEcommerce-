namespace Catalog.Domain.Entities;

public class Category : BaseEntity
{
    private readonly List<Category> _children = new();

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public IReadOnlyList<Category> Children => _children.AsReadOnly();
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Category() { }

    public static Category Create(string name, string slug, string? description = null, Guid? parentId = null, int displayOrder = 0)
        => new()
        {
            Name = name,
            Slug = slug,
            Description = description,
            ParentId = parentId,
            DisplayOrder = displayOrder,
            IsActive = true
        };

    public void Update(string name, string slug, string? description, int displayOrder)
    {
        Name = name;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
        SetUpdatedAt();
    }

    public void AddChild(Category child)
    {
        child.ParentId = Id;
        _children.Add(child);
    }
}
