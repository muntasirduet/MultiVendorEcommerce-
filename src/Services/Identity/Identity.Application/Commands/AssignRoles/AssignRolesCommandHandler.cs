using Identity.Application.Common;
using Identity.Application.Interfaces;
using Identity.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.Application.Commands.AssignRoles;

public class AssignRolesCommandHandler : IRequestHandler<AssignRolesCommand, Result<bool>>
{
    private readonly IUserProfileRepository _repo;
    private readonly IKeycloakService _keycloak;
    private readonly IPublishEndpoint _bus;

    public AssignRolesCommandHandler(IUserProfileRepository repo, IKeycloakService keycloak, IPublishEndpoint bus)
    {
        _repo = repo;
        _keycloak = keycloak;
        _bus = bus;
    }

    public async Task<Result<bool>> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var profile = await _repo.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result<bool>.Failure(Error.NotFound("User"));

        try
        {
            var currentRoles = await _keycloak.GetUserRolesAsync(profile.KeycloakId, cancellationToken);

            foreach (var role in request.Roles)
                await _keycloak.AssignRoleAsync(profile.KeycloakId, role, cancellationToken);

            var allRoles = currentRoles.Union(request.Roles).Distinct().ToArray();

            await _bus.Publish(new UserRoleChangedEvent
            {
                UserId = profile.Id,
                Email = profile.Email,
                NewRoles = allRoles
            }, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Error.Conflict(ex.Message));
        }
    }
}
