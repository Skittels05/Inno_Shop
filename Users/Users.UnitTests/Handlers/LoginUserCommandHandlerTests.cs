using FluentAssertions;
using Moq;
using Users.Application.CQRS.Commands;
using Users.Application.CQRS.Handlers;
using Users.Application.DTOs;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Identity;
using Users.Application.Interfaces.Repositories;
using Users.Domain.Entities;
using Users.Domain.Enums;
using MediatR;
using Xunit;

namespace Users.UnitTests.Handlers;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _handler = new LoginUserCommandHandler(
            _repositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResult_WhenValidCredentials()
    {
        var loginDto = new UserLoginDto("test@example.com", "Password123");
        var command = new LoginUserCommand(loginDto);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            PasswordHash = "hashed_password",
            Role = Role.User
        };

        var token = "jwt_token";

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, false))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(loginDto.Password, user.PasswordHash))
            .Returns(true);
        _jwtTokenGeneratorMock.Setup(g => g.GenerateToken(user.Id, user.Email, user.Role.ToString()))
            .Returns(token);

        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.UserId.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be(user.Role.ToString());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenInvalidCredentials()
    {
        var loginDto = new UserLoginDto("test@example.com", "WrongPassword");
        var command = new LoginUserCommand(loginDto);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            PasswordHash = "hashed_password"
        };

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, false))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(loginDto.Password, user.PasswordHash))
            .Returns(false);
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenUserNotFound()
    {

        var loginDto = new UserLoginDto("nonexistent@example.com", "Password123");
        var command = new LoginUserCommand(loginDto);

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, false))
            .ReturnsAsync((User?)null);
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

