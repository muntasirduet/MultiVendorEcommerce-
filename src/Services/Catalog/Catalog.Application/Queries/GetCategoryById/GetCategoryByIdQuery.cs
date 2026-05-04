using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid CategoryId) : IRequest<Result<CategoryDto>>;
