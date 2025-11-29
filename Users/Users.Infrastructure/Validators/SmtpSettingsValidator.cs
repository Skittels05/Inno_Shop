using FluentValidation;
using Users.Infrastructure.Models;

namespace Users.Infrastructure.Validators
{
    public class SmtpSettingsValidator : AbstractValidator<SmtpSettings>
    {
        public SmtpSettingsValidator()
        {
            RuleFor(x => x.Server)
                .NotEmpty().WithMessage("SMTP server is required")
                .Matches(@"^[a-zA-Z0-9.-]+$").WithMessage("Invalid SMTP server format");

            RuleFor(x => x.Port)
                .InclusiveBetween(1, 65535).WithMessage("SMTP port must be between 1 and 65535");

            RuleFor(x => x.SenderEmail)
                .NotEmpty().WithMessage("Sender email is required")
                .EmailAddress().WithMessage("Invalid sender email format");

            RuleFor(x => x.SenderName)
                .NotEmpty().WithMessage("Sender name is required")
                .MaximumLength(100).WithMessage("Sender name must not exceed 100 characters");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("SMTP username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("SMTP password is required");
        }
    }
}