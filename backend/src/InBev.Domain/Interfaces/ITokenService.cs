using InBev.Domain.Entities;

namespace InBev.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(Employee employee);
}

