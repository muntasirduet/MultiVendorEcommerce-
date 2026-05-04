using Identity.Application.Common;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IKeycloakService _keycloak;

    public LogoutCommandHandler(IKeycloakService keycloak) => _keycloak = keycloak;

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _keycloak.RevokeTokenAsync(request.RefreshToken, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Error.Conflict(ex.Message));
        }
    }
}
