using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    Guid? ParentId,
    int DisplayOrder) : IRequest<Result<Guid>>;
