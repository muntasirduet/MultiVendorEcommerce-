using System.Net.Http.Json;
using System.Text.Json;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Keycloak;

public class KeycloakAdminClient : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;

    public KeycloakAdminClient(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    private async Task<string> GetAdminTokenAsync(CancellationToken ct = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _settings.AdminClientId,
            ["client_secret"] = _settings.AdminClientSecret
        });

        var response = await _httpClient.PostAsync(
            $"/realms/{_settings.Realm}/protocol/openid-connect/token", content, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return json.GetProperty("access_token").GetString()
               ?? throw new InvalidOperationException("Failed to obtain admin access token.");
    }

    public async Task<string> CreateUserAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default)
    {
        var adminToken = await GetAdminTokenAsync(ct);

        var request = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_settings.Realm}/users")
        {
            Headers = { { "Authorization", $"Bearer {adminToken}" } },
            Content = JsonContent.Create(new
            {
                username = email,
                email,
                firstName,
                lastName,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = password, temporary = false }
                }
            })
        };

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString()
                       ?? throw new InvalidOperationException("Keycloak did not return a user location.");

        return location.Split('/').Last();
    }

    public async Task AssignRoleAsync(string keycloakUserId, string role, CancellationToken ct = default)
    {
        var adminToken = await GetAdminTokenAsync(ct);

        // Get role representation
        var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_settings.Realm}/roles/{role}");
        roleRequest.Headers.Add("Authorization", $"Bearer {adminToken}");
        var roleResponse = await _httpClient.SendAsync(roleRequest, ct);
        roleResponse.EnsureSuccessStatusCode();
        var roleJson = await roleResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

        // Assign role to user
        var assignRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm")
        {
            Headers = { { "Authorization", $"Bearer {adminToken}" } },
            Content = JsonContent.Create(new[] { roleJson })
        };

        var assignResponse = await _httpClient.SendAsync(assignRequest, ct);
        assignResponse.EnsureSuccessStatusCode();
    }

    public async Task<TokenResponse> GetTokenAsync(string email, string password, CancellationToken ct = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _settings.AdminClientId,
            ["client_secret"] = _settings.AdminClientSecret,
            ["username"] = email,
            ["password"] = password
        });

        var response = await _httpClient.PostAsync(
            $"/realms/{_settings.Realm}/protocol/openid-connect/token", content, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return new TokenResponse(
            AccessToken: json.GetProperty("access_token").GetString() ?? string.Empty,
            RefreshToken: json.GetProperty("refresh_token").GetString() ?? string.Empty,
            ExpiresIn: json.GetProperty("expires_in").GetInt32());
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _settings.AdminClientId,
            ["client_secret"] = _settings.AdminClientSecret,
            ["refresh_token"] = refreshToken
        });

        var response = await _httpClient.PostAsync(
            $"/realms/{_settings.Realm}/protocol/openid-connect/token", content, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return new TokenResponse(
            AccessToken: json.GetProperty("access_token").GetString() ?? string.Empty,
            RefreshToken: json.GetProperty("refresh_token").GetString() ?? string.Empty,
            ExpiresIn: json.GetProperty("expires_in").GetInt32());
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _settings.AdminClientId,
            ["client_secret"] = _settings.AdminClientSecret,
            ["token"] = refreshToken
        });

        var response = await _httpClient.PostAsync(
            $"/realms/{_settings.Realm}/protocol/openid-connect/revoke", content, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string[]> GetUserRolesAsync(string keycloakUserId, CancellationToken ct = default)
    {
        var adminToken = await GetAdminTokenAsync(ct);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm");
        request.Headers.Add("Authorization", $"Bearer {adminToken}");

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken: ct);
        if (json is null) return [];

        return json
            .Where(r => r.TryGetProperty("name", out _))
            .Select(r => r.GetProperty("name").GetString() ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToArray();
    }
}
