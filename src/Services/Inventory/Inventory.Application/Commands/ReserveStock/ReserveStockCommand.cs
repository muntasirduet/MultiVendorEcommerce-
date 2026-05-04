using Inventory.Application.Common;
using MediatR;

namespace Inventory.Application.Commands.ReserveStock;

public record ReserveStockCommand(Guid OrderId, Guid ProductId, int Quantity, int TtlMinutes = 15) : IRequest<Result<Guid>>;
