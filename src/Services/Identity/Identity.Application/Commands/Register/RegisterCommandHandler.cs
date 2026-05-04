using Identity.Application.Common;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Events;
using MassTransit;
using MediatR;

namespace Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IUserProfileRepository _repo;
    private readonly IKeycloakService _keycloak;
    private readonly IPublishEndpoint _bus;

    public RegisterCommandHandler(IUserProfileRepository repo, IKeycloakService keycloak, IPublishEndpoint bus)
    {
        _repo = repo;
        _keycloak = keycloak;
        _bus = bus;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure(Error.Conflict("A user with this email already exists."));

        try
        {
            var keycloakId = await _keycloak.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

            await _keycloak.AssignRoleAsync(keycloakId, request.Role, cancellationToken);

            var profile = UserProfile.Create(
                keycloakId, request.Email, request.FirstName, request.LastName, request.PhoneNumber);

            await _repo.AddAsync(profile, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            await _bus.Publish(new UserRegisteredEvent
            {
                UserId = profile.Id,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName
            }, cancellationToken);

            return Result<Guid>.Success(profile.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(Error.Conflict(ex.Message));
        }
    }
}
