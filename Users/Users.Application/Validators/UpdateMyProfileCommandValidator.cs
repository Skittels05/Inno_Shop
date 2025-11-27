using FluentValidation;
using Users.Application.CQRS.Commands;

namespace Users.Application.Validators;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Dto.Name)
            .MaximumLength(100).When(x => x.Dto.Name != null);

        RuleFor(x => x.Dto.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).When(x => x.Dto.Email != null);
    }
}