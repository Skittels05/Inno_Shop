using FluentValidation;
using Users.Application.CQRS.Commands;

namespace Users.Application.Validators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}