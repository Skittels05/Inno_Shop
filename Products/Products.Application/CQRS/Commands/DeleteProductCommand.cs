using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Commands
{
    public record DeleteProductCommand(Guid ProductId, Guid UserId) : IRequest<Unit>;
}
