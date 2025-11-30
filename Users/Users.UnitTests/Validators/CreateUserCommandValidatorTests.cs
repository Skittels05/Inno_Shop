using FluentAssertions;
using Users.Application.CQRS.Commands;
using Users.Application.Validators;
using Xunit;

namespace Users.UnitTests.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenValidCommand()
    {

        var command = new CreateUserCommand("John Doe", "john@example.com", "Password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {

        var command = new CreateUserCommand("", "john@example.com", "Password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && 
                                           e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var command = new CreateUserCommand("John Doe", "invalid-email", "Password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && 
                                           e.ErrorMessage == "A valid email is required");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "12345");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && 
                                           e.ErrorMessage == "Password must be at least 6 characters");
    }
}

