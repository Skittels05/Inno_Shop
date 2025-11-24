using FluentValidation;
using Users.Application.CQRS.Commands;

namespace Users.Application.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.ResetDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.ResetDto.Token)
                .NotEmpty().WithMessage("Token is required");

            RuleFor(x => x.ResetDto.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}