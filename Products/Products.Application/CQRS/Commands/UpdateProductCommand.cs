using MediatR;
using Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Commands
{
    public record UpdateProductCommand(Guid ProductId, UpdateProductDto Dto, Guid UserId) : IRequest;
}
