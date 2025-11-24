using FluentValidation;
using Users.Application.CQRS.Commands;

namespace Users.Application.Validators
{
    public class PasswordRecoveryCommandValidator : AbstractValidator<PasswordRecoveryCommand>
    {
        public PasswordRecoveryCommandValidator()
        {
            RuleFor(x => x.RecoveryDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");
        }
    }
}