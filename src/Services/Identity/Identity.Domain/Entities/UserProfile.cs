using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string KeycloakId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;

    private UserProfile() { }

    public static UserProfile Create(string keycloakId, string email, string firstName, string lastName, string? phoneNumber = null)
        => new() { KeycloakId = keycloakId, Email = email, FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber };

    public void UpdateStatus(UserStatus status) { Status = status; SetUpdatedAt(); }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        SetUpdatedAt();
    }
}
