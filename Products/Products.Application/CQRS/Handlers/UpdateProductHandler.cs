using AutoMapper;
using MediatR;
using Products.Application.CQRS.Commands;
using Products.Application.Exceptions;
using Products.Domain.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public UpdateProductHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, request.UserId);

            if (product == null)
                throw new NotFoundException("Product", request.ProductId);

            if (product.IsDeleted)
                throw new BadRequestException("Cannot update a deleted product.");

            if (request.Dto.Name != null)
                product.Name = request.Dto.Name;

            if (request.Dto.Description != null)
                product.Description = request.Dto.Description;

            if (request.Dto.IsAvailable.HasValue)
                product.IsAvailable = request.Dto.IsAvailable.Value;

            await _repository.UpdateAsync(product);
        }
    }
}