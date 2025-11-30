using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Products.IntegrationTests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMyProducts_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        var dto = new CreateProductDto("Test Product", "Test Description", true);
        var response = await _client.PostAsJsonAsync("/api/products", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

