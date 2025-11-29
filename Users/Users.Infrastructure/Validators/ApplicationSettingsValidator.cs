using FluentValidation;
using Users.Infrastructure.Models;

namespace Users.Infrastructure.Validators
{
    public class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
    {
        public ApplicationSettingsValidator()
        {
            RuleFor(x => x.BaseUrl)
                .NotEmpty().WithMessage("Base URL is required")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Invalid base URL format");

            RuleFor(x => x.ConfirmEmailPath)
                .NotEmpty().WithMessage("Confirm email path is required");

            RuleFor(x => x.ResetPasswordPath)
                .NotEmpty().WithMessage("Reset password path is required");
        }
    }
}