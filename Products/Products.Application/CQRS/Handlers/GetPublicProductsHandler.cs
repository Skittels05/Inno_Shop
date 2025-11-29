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
    public class GetPublicProductsHandler : IRequestHandler<GetPublicProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetPublicProductsHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetPublicProductsQuery request, CancellationToken cancellationToken)
        {
            var productsQuery = _repository.FindAll();

            if (request.Filter != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Filter.Name))
                {
                    productsQuery = productsQuery.Where(p => p.Name.Contains(request.Filter.Name));
                }

                if (!string.IsNullOrWhiteSpace(request.Filter.Description))
                {
                    productsQuery = productsQuery.Where(p => p.Description.Contains(request.Filter.Description));
                }

                if (request.Filter.IsAvailable.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.IsAvailable == request.Filter.IsAvailable.Value);
                }
            }

            var products = await Task.Run(() => productsQuery.ToList(), cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}