using FluentValidation;

namespace Catalog.Application.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(200).WithMessage("Category name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Category slug is required.")
            .MaximumLength(200).WithMessage("Category slug must not exceed 200 characters.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.");
    }
}
