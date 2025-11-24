using MediatR;

namespace Users.Application.CQRS.Commands
{
    public record ConfirmEmailCommand(string Email, string Token) : IRequest<bool>;
}