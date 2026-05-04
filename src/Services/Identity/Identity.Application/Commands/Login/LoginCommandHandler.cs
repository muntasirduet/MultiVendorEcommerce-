using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IKeycloakService _keycloak;

    public LoginCommandHandler(IKeycloakService keycloak) => _keycloak = keycloak;

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _keycloak.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return Result<TokenResponse>.Success(token);
        }
        catch (Exception ex)
        {
            return Result<TokenResponse>.Failure(Error.Unauthorized(ex.Message));
        }
    }
}
