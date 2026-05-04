using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserProfile?> GetByKeycloakIdAsync(string keycloakId, CancellationToken ct = default);
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(UserProfile profile, CancellationToken ct = default);
    Task UpdateAsync(UserProfile profile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
