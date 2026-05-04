using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;
