using FluentValidation;
using Users.Application.CQRS.Commands;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.LoginDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.LoginDto.Password)
                .NotEmpty().WithMessage("Password is required");

            RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    var user = await userRepository.GetByEmailAsync(command.LoginDto.Email, false);
                    if (user == null) return true;
                    return user.IsActive;
                })
                .WithMessage("Account is inactive");

            RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    var user = await userRepository.GetByEmailAsync(command.LoginDto.Email, false);
                    if (user == null) return true; 
                    return user.EmailConfirmed;
                })
                .WithMessage("Email is not confirmed");
        }
    }
}
