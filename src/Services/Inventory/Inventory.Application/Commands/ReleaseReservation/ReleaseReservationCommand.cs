using Inventory.Application.Common;
using MediatR;

namespace Inventory.Application.Commands.ReleaseReservation;

public record ReleaseReservationCommand(Guid OrderId, Guid ProductId) : IRequest<Result<bool>>;
