using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IKeycloakService _keycloak;

    public RefreshTokenCommandHandler(IKeycloakService keycloak) => _keycloak = keycloak;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _keycloak.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Result<TokenResponse>.Success(token);
        }
        catch (Exception ex)
        {
            return Result<TokenResponse>.Failure(Error.Unauthorized(ex.Message));
        }
    }
}
