using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryTreeDto>>>;
