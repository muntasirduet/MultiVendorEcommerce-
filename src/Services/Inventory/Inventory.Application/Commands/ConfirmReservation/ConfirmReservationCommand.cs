using Inventory.Application.Common;
using MediatR;

namespace Inventory.Application.Commands.ConfirmReservation;

public record ConfirmReservationCommand(Guid OrderId, Guid ProductId) : IRequest<Result<bool>>;
