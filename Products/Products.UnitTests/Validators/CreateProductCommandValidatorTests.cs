using FluentAssertions;
using Products.Application.CQRS.Commands;
using Products.Application.DTOs;
using Products.Application.Validators;
using Xunit;

namespace Products.UnitTests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenValidCommand()
    {

        var userId = Guid.NewGuid();
        var dto = new CreateProductDto("Valid Product Name", "Valid Description", true);
        var command = new CreateProductCommand(dto, userId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateProductDto("", "Description", true);
        var command = new CreateProductCommand(dto, userId);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dto.Name" && 
                                           e.ErrorMessage == "Product name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameExceedsMaxLength()
    {

        var userId = Guid.NewGuid();
        var longName = new string('A', 201);
        var dto = new CreateProductDto(longName, "Description", true);
        var command = new CreateProductCommand(dto, userId);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dto.Name" && 
                                           e.ErrorMessage == "Product name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionExceedsMaxLength()
    {

        var userId = Guid.NewGuid();
        var longDescription = new string('A', 1001);
        var dto = new CreateProductDto("Valid Name", longDescription, true);
        var command = new CreateProductCommand(dto, userId);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dto.Description" && 
                                           e.ErrorMessage == "Description must not exceed 1000 characters.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenUserIdIsEmpty()
    {

        var dto = new CreateProductDto("Valid Name", "Description", true);
        var command = new CreateProductCommand(dto, Guid.Empty);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId" && 
                                           e.ErrorMessage == "User ID is required.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenDescriptionIsNull()
    {

        var userId = Guid.NewGuid();
        var dto = new CreateProductDto("Valid Name", null!, true);
        var command = new CreateProductCommand(dto, userId);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}

