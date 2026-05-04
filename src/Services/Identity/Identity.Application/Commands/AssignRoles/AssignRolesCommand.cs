using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Commands.AssignRoles;

public record AssignRolesCommand(Guid UserId, string[] Roles) : IRequest<Result<bool>>;
