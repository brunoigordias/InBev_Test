using FluentAssertions;
using InBev.Application.DTOs;
using InBev.Application.Validators;
using InBev.Domain.Enums;
using Xunit;

namespace InBev.UnitTests.Application.Validators;

public class CreateEmployeeValidatorTests
{
    private readonly CreateEmployeeValidator _validator;

    public CreateEmployeeValidatorTests()
    {
        _validator = new CreateEmployeeValidator();
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.FirstName = string.Empty;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.LastName = string.Empty;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.Email = email;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    public void Should_Have_Error_When_CPF_Is_Invalid(string cpf)
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.DocNumber = cpf;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DocNumber");
    }

    [Fact]
    public void Should_Accept_Valid_CPF()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.DocNumber = "12345678909"; // CPF válido

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("weak")]
    public void Should_Have_Error_When_Password_Is_Weak(string password)
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.Password = password;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Should_Have_Error_When_Employee_Is_Minor()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.BirthDate = DateTime.Today.AddYears(-17); // Menor de idade

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BirthDate");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumbers_Is_Empty()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();
        dto.PhoneNumbers = new List<CreatePhoneNumberDto>();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumbers");
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        // Arrange
        var dto = CreateValidEmployeeDto();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    private CreateEmployeeDto CreateValidEmployeeDto()
    {
        return new CreateEmployeeDto
        {
            FirstName = "João",
            LastName = "Silva",
            Email = "joao.silva@example.com",
            DocNumber = "12345678909",
            Password = "Password123",
            BirthDate = DateTime.Today.AddYears(-25),
            Role = EmployeeRole.Employee,
            PhoneNumbers = new List<CreatePhoneNumberDto>
            {
                new() { Number = "(11) 98765-4321", Type = PhoneType.Mobile }
            }
        };
    }
}

