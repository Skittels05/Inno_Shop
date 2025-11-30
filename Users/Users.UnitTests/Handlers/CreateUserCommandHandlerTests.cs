using AutoMapper;
using FluentAssertions;
using Moq;
using Users.Application.CQRS.Commands;
using Users.Application.Interfaces.Identity;
using Users.Application.Interfaces.Repositories;
using Users.Domain.Entities;
using MediatR;
using Xunit;

namespace Users.UnitTests.Handlers;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(
            _repositoryMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidCommand()
    {
        var command = new CreateUserCommand("Test User", "test@example.com", "Password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email
        };
        var hashedPassword = "hashed_password";

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _passwordHasherMock.Setup(h => h.Hash(command.Password)).Returns(hashedPassword);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().Be(user.Id);
        _passwordHasherMock.Verify(h => h.Hash(command.Password), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.Is<User>(u => u.PasswordHash == hashedPassword)), Times.Once);
    }
}

