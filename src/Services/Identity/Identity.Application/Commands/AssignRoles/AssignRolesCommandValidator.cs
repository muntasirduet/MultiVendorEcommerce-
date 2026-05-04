using FluentValidation;

namespace Identity.Application.Commands.AssignRoles;

public class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    private static readonly string[] ValidRoles = { "admin", "vendor", "customer" };

    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Roles)
            .NotEmpty()
            .WithMessage("At least one role must be specified.");

        RuleForEach(x => x.Roles)
            .Must(r => ValidRoles.Contains(r))
            .WithMessage("Each role must be admin, vendor, or customer.");
    }
}
