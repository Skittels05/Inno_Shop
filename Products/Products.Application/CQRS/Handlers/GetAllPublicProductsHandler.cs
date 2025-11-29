using AutoMapper;
using MediatR;
using Products.Application.CQRS.Queries;
using Products.Application.DTOs;
using Products.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application.CQRS.Handlers
{
    public class GetAllPublicProductsHandler : IRequestHandler<GetAllPublicProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPublicProductsHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllPublicProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await Task.Run(() => _repository.FindAll().ToList(), cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}