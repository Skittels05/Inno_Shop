using MediatR;
using Products.Application.CQRS.Commands;
using Products.Application.Exceptions;
using Products.Domain.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _repository;

        public DeleteProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, request.UserId);

            if (product == null)
                throw new NotFoundException("Product", request.ProductId);

            if (product.IsDeleted)
                throw new BadRequestException("Product is already deleted.");

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product);

            return Unit.Value;
        }
    }
}