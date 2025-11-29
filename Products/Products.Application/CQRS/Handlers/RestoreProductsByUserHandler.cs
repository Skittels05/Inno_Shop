using MediatR;
using Products.Application.CQRS.Commands;
using Products.Domain.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class RestoreProductsByUserHandler : IRequestHandler<RestoreProductsByUserCommand>
    {
        private readonly IProductRepository _repository;

        public RestoreProductsByUserHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreProductsByUserCommand request, CancellationToken cancellationToken)
        {

            var products = (await _repository.GetProductsByUserAsync(request.UserId, includeDeleted: true))
                .Where(p => p.IsDeleted)
                .ToList();

            if (!products.Any())
                return;

            foreach (var product in products)
            {
                product.IsDeleted = false;
                product.IsAvailable = true;
                product.DeletedAt = null;
            }

            await _repository.UpdateRangeAsync(products);
        }
    }
}