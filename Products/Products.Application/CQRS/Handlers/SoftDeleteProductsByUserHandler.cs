using MediatR;
using Products.Application.CQRS.Commands;
using Products.Domain.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class SoftDeleteProductsByUserHandler : IRequestHandler<SoftDeleteProductsByUserCommand>
    {
        private readonly IProductRepository _repository;

        public SoftDeleteProductsByUserHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteProductsByUserCommand request, CancellationToken cancellationToken)
        {
            var products = (await _repository.GetProductsByUserAsync(request.UserId, includeDeleted: false))
                .ToList();

            if (!products.Any())
                return;

            var utcNow = DateTime.UtcNow;
            foreach (var product in products)
            {
                product.IsDeleted = true;
                product.IsAvailable = false;
                product.DeletedAt = utcNow;
            }

            await _repository.UpdateRangeAsync(products);
        }
    }
}