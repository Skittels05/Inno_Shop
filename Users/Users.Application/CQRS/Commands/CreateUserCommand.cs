using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Enums;

namespace Users.Application.CQRS.Commands
{
    public record CreateUserCommand(
        string Name,
        string Email,
        string Password
    ) : IRequest<Guid>;
}
