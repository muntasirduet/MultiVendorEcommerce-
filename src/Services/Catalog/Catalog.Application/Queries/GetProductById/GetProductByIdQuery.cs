using Catalog.Application.Common;
using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDto>>;
