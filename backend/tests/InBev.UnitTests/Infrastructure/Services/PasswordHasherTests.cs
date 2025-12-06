using FluentAssertions;
using InBev.Infrastructure.Services;
using Xunit;

namespace InBev.UnitTests.Infrastructure.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_Should_Return_Non_Empty_Hash()
    {
        // Arrange
        var password = "MyPassword123";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void HashPassword_Should_Generate_Different_Hashes_For_Same_Password()
    {
        // Arrange
        var password = "MyPassword123";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt gera salt único
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var password = "MyPassword123";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Incorrect_Password()
    {
        // Arrange
        var password = "MyPassword123";
        var wrongPassword = "WrongPassword456";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void VerifyPassword_Should_Return_False_For_Empty_Password(string emptyPassword)
    {
        // Arrange
        var password = "MyPassword123";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(emptyPassword, hash);

        // Assert
        result.Should().BeFalse();
    }
}

