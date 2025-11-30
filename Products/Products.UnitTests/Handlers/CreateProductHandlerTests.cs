using AutoMapper;
using FluentAssertions;
using Moq;
using Products.Application.CQRS.Commands;
using Products.Application.CQRS.Handlers;
using Products.Application.DTOs;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using Xunit;

namespace Products.UnitTests.Handlers;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProductHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenValidCommand()
    {
        var userId = Guid.NewGuid();
        var createDto = new CreateProductDto("Test Product", "Test Description", true);
        var command = new CreateProductCommand(createDto, userId);
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            IsAvailable = createDto.IsAvailable,
            UserId = userId
        };

        var productDto = new ProductDto(product.Id, product.Name, product.Description, product.IsAvailable);

        _mapperMock.Setup(m => m.Map<Product>(createDto)).Returns(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Description.Should().Be(createDto.Description);
        result.IsAvailable.Should().Be(createDto.IsAvailable);
        
        _mapperMock.Verify(m => m.Map<Product>(createDto), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.Is<Product>(p => p.UserId == userId)), Times.Once);
        _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetUserId_WhenCreatingProduct()
    {

        var userId = Guid.NewGuid();
        var createDto = new CreateProductDto("Test Product", "Test Description", true);
        var command = new CreateProductCommand(createDto, userId);
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            IsAvailable = createDto.IsAvailable
        };

        _mapperMock.Setup(m => m.Map<Product>(createDto)).Returns(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDto(p.Id, p.Name, p.Description, p.IsAvailable));
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.Is<Product>(p => p.UserId == userId)), Times.Once);
    }
}

