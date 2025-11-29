using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.CQRS.Commands;
using Products.Application.CQRS.Queries;
using Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Products.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;

        private Guid CurrentUserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetMyProducts()
        {
            var products = await _mediator.Send(new GetMyProductsQuery(CurrentUserId));
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            var product = await _mediator.Send(new CreateProductCommand(dto, CurrentUserId));
            return Ok(product);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            await _mediator.Send(new UpdateProductCommand(id, dto, CurrentUserId));
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}
