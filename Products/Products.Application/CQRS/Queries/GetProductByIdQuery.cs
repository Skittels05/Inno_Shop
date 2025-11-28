using MediatR;
using Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Queries
{
    public record GetProductByIdQuery(Guid ProductId, Guid UserId) : IRequest<ProductDto>;
}
