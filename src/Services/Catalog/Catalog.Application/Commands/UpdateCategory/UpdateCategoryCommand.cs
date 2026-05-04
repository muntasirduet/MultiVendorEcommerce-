using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder) : IRequest<Result<bool>>;
