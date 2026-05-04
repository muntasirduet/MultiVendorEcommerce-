using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IdentityDbContext _context;

    public UserProfileRepository(IdentityDbContext context) => _context = context;

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<UserProfile?> GetByKeycloakIdAsync(string keycloakId, CancellationToken ct = default)
        => await _context.UserProfiles.FirstOrDefaultAsync(u => u.KeycloakId == keycloakId, ct);

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(UserProfile profile, CancellationToken ct = default)
        => await _context.UserProfiles.AddAsync(profile, ct);

    public Task UpdateAsync(UserProfile profile, CancellationToken ct = default)
    {
        _context.UserProfiles.Update(profile);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
