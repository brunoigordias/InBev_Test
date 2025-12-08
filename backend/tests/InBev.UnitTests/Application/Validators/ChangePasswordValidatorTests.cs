using FluentAssertions;
using InBev.Application.DTOs;
using InBev.Application.Validators;
using Xunit;

namespace InBev.UnitTests.Application.Validators;

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _validator;

    public ChangePasswordValidatorTests()
    {
        _validator = new ChangePasswordValidator();
    }

    [Fact]
    public void Should_Have_Error_When_CurrentPassword_Is_Empty()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "",
            NewPassword = "newPassword123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(dto.CurrentPassword));
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Is_Empty()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "currentPassword123",
            NewPassword = ""
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(dto.NewPassword));
    }

    [Theory]
    [InlineData("12345")]    // 5 characters
    [InlineData("abc")]      // 3 characters
    [InlineData("1")]        // 1 character
    public void Should_Have_Error_When_NewPassword_Is_Too_Short(string newPassword)
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "currentPassword123",
            NewPassword = newPassword
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(dto.NewPassword) 
            && x.ErrorMessage.Contains("no mínimo 6 caracteres"));
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Is_Same_As_CurrentPassword()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "samePassword123",
            NewPassword = "samePassword123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage.Contains("deve ser diferente"));
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "currentPassword123",
            NewPassword = "newPassword456"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Accept_NewPassword_With_Exactly_6_Characters()
    {
        // Arrange
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "oldPass",
            NewPassword = "123456"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

