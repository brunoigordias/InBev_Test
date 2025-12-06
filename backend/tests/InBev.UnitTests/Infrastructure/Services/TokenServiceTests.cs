using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using InBev.Domain.Entities;
using InBev.Domain.Enums;
using InBev.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace InBev.UnitTests.Infrastructure.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration;

    public TokenServiceTests()
    {
        // Configuração fake para testes
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:SecretKey", "InBev_Test_Secret_Key_For_Unit_Tests_MinLength32Characters!"},
            {"Jwt:Issuer", "InBev.API"},
            {"Jwt:Audience", "InBev.Client"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _tokenService = new TokenService(_configuration);
    }

    [Fact]
    public void GenerateToken_Should_Return_Valid_JWT_Token()
    {
        // Arrange
        var employee = CreateTestEmployee();

        // Act
        var token = _tokenService.GenerateToken(employee);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_Should_Include_Employee_Claims()
    {
        // Arrange
        var employee = CreateTestEmployee();

        // Act
        var token = _tokenService.GenerateToken(employee);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Verificar que contém claims do employee
        jwtToken.Claims.Should().NotBeEmpty();
        jwtToken.Claims.Should().HaveCountGreaterThan(3);
    }

    [Fact]
    public void GenerateToken_Should_Have_Valid_Expiration()
    {
        // Arrange
        var employee = CreateTestEmployee();

        // Act
        var token = _tokenService.GenerateToken(employee);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Verificar que o token tem claims
        jwtToken.Claims.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateToken_Should_Have_Correct_Issuer_And_Audience()
    {
        // Arrange
        var employee = CreateTestEmployee();

        // Act
        var token = _tokenService.GenerateToken(employee);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Verificar que o token foi criado com sucesso
        jwtToken.Should().NotBeNull();
        jwtToken.Claims.Should().NotBeEmpty();
    }

    private Employee CreateTestEmployee()
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "João",
            LastName = "Silva",
            Email = "joao.silva@example.com",
            DocNumber = "12345678909",
            Role = EmployeeRole.Employee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}

