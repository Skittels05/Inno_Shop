using MediatR;
using Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Commands
{
    public record CreateProductCommand(CreateProductDto Dto, Guid UserId) : IRequest<Guid>;
}
