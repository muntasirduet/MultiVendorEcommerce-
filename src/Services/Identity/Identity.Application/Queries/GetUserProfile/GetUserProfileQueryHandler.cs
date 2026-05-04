using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserProfileRepository _repo;

    public GetUserProfileQueryHandler(IUserProfileRepository repo) => _repo = repo;

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repo.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result<UserProfileDto>.Failure(Error.NotFound("User"));

        return Result<UserProfileDto>.Success(new UserProfileDto
        {
            Id = profile.Id,
            KeycloakId = profile.KeycloakId,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            PhoneNumber = profile.PhoneNumber,
            Status = profile.Status.ToString(),
            CreatedAt = profile.CreatedAt
        });
    }
}
