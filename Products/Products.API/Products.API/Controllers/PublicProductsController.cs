using MediatR;
using Microsoft.AspNetCore.Mvc;
using Products.Application.CQRS.Queries;
using Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/public/products")]
    public class PublicProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _mediator.Send(new GetAllPublicProductsQuery());
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? description = null,
            [FromQuery] bool? isAvailable = null)
        {
            var filter = new ProductFilterDto(name, description, isAvailable);
            var products = await _mediator.Send(new SearchPublicProductsQuery(filter));
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}