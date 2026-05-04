using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role = "customer") : IRequest<Result<Guid>>;
