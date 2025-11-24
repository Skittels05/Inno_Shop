using MediatR;
using Users.Application.DTOs;

namespace Users.Application.CQRS.Commands
{
    public record LoginUserCommand(UserLoginDto LoginDto) : IRequest<AuthResultDto>;
}
