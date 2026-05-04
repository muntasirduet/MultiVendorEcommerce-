using Inventory.Application.Common;
using MediatR;

namespace Inventory.Application.Commands.UpdateStock;

public record UpdateStockCommand(Guid ProductId, Guid VendorId, int Quantity, int LowStockThreshold = 5) : IRequest<Result<bool>>;
