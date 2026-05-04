using Identity.Application.DTOs;

namespace Identity.Application.Interfaces;

public interface IKeycloakService
{
    Task<string> CreateUserAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default);
    Task AssignRoleAsync(string keycloakUserId, string role, CancellationToken ct = default);
    Task<TokenResponse> GetTokenAsync(string email, string password, CancellationToken ct = default);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<string[]> GetUserRolesAsync(string keycloakUserId, CancellationToken ct = default);
}
