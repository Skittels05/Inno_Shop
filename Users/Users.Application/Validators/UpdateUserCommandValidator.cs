using FluentValidation;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;

namespace Users.Application.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.UserDto.Name)
                .NotEmpty().WithMessage("Name is required")
                .When(x => x.UserDto.Name != null)
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.UserDto.Email)
                .EmailAddress().WithMessage("A valid email is required")
                .When(x => !string.IsNullOrEmpty(x.UserDto.Email))
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
        }
    }
}