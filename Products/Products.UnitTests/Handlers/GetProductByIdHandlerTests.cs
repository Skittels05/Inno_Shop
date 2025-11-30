using AutoMapper;
using FluentAssertions;
using Moq;
using Products.Application.CQRS.Handlers;
using Products.Application.CQRS.Queries;
using Products.Application.DTOs;
using Products.Application.Exceptions;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using System.Linq;
using Xunit;

namespace Products.UnitTests.Handlers;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProductByIdHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            IsAvailable = true
        };

        var productDto = new ProductDto(product.Id, product.Name, product.Description, product.IsAvailable);
        var query = new GetProductByIdQuery(productId);

        var mockQueryable = new List<Product> { product }.AsQueryable();
        _repositoryMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<bool>()))
            .Returns(mockQueryable);
        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be(product.Name);
        _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        var query = new GetProductByIdQuery(productId);

        var emptyQueryable = new List<Product>().AsQueryable();
        _repositoryMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<bool>()))
            .Returns(emptyQueryable);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}

