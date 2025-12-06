using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InBev.Domain.Entities;
using InBev.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InBev.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Employee employee)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured"));
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Email, employee.Email),
            new(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
            new(ClaimTypes.Role, employee.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

