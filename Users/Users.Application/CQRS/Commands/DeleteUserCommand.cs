using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.CQRS.Commands
{
    public record DeleteUserCommand(Guid Id) : IRequest<Unit>;
}
