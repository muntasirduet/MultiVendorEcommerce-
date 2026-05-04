namespace Identity.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; init; }
    public string KeycloakId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record AssignRolesRequest(string[] Roles);
