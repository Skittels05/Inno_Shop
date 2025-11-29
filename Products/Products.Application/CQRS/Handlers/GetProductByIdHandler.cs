using AutoMapper;
using MediatR;
using Products.Application.CQRS.Queries;
using Products.Application.DTOs;
using Products.Domain.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, request.UserId);

            if (product == null)
                return null;

            return _mapper.Map<ProductDto>(product);
        }
    }
}