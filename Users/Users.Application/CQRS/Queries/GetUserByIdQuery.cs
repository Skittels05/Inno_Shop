using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTOs;

namespace Users.Application.CQRS.Queries
{
    public record GetUserByIdQuery(Guid Id, bool TrackChanges = false) : IRequest<UserDto>;
}
