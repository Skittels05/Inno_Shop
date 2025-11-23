using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTOs;

namespace Users.Application.CQRS.Commands
{
    public record UpdateUserCommand(
        Guid Id,
        UpdateUserDto UserDto
    ) : IRequest<Unit>;
}
