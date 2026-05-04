using Inventory.Application.Common;
using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Queries.GetInventoryByProduct;

public record GetInventoryByProductQuery(Guid ProductId) : IRequest<Result<InventoryItemDto>>;
