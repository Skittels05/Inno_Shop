using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Users.Application.DTOs;
using Xunit;

namespace Users.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenValidData()
    {
        var createUserDto = new CreateUserDto(
            Name: "Test User",
            Email: $"test_{Guid.NewGuid()}@example.com",
            Password: "Password123"
        );

        var response = await _client.PostAsJsonAsync("/api/auth/register", createUserDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenInvalidData()
    {

        var createUserDto = new CreateUserDto(
            Name: "",
            Email: "invalid-email",
            Password: "123"
        );

        var response = await _client.PostAsJsonAsync("/api/auth/register", createUserDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {

        var email = "nonexistent@example.com";
        var password = "WrongPassword";
        var response = await _client.GetAsync($"/api/auth/login?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }
}

