using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Products.Application.CQRS.Commands
{
    public record RestoreProductsByUserCommand(Guid UserId) : IRequest;
}
