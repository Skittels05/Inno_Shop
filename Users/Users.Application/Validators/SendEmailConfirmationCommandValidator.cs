using FluentValidation;
using Users.Application.CQRS.Commands;

namespace Users.Application.Validators
{
    public class SendEmailConfirmationCommandValidator : AbstractValidator<SendEmailConfirmationCommand>
    {
        public SendEmailConfirmationCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");
        }
    }
}