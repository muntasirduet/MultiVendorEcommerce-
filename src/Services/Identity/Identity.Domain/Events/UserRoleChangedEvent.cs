namespace Identity.Domain.Events;

public record UserRoleChangedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType => nameof(UserRoleChangedEvent);
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string[] NewRoles { get; init; }
}
