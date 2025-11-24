using MediatR;

namespace Users.Application.CQRS.Commands
{
    public record SendEmailConfirmationCommand(string Email) : IRequest<Unit>;
}