using InBev.Domain.Entities;
using InBev.Domain.Enums;
using InBev.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InBev.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Garantir que o banco de dados está criado
            await _context.Database.MigrateAsync();

            // Verificar se já existe algum usuário administrador
            var adminExists = await _context.Employees
                .AnyAsync(e => e.Email == "admin@inbev.com");

            if (!adminExists)
            {
                _logger.LogInformation("Criando usuário administrador padrão...");

                var adminEmployee = new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Email = "admin@inbev.com",
                    DocNumber = "12345678909", // CPF válido de exemplo
                    PasswordHash = _passwordHasher.HashPassword("Admin@123"),
                    BirthDate = new DateTime(1980, 1, 1),
                    Role = EmployeeRole.Manager,
                    ManagerId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    PhoneNumbers = new List<PhoneNumber>
                    {
                        new PhoneNumber
                        {
                            Id = Guid.NewGuid(),
                            Number = "(11) 99999-9999",
                            Type = PhoneType.Mobile
                        },
                        new PhoneNumber
                        {
                            Id = Guid.NewGuid(),
                            Number = "(11) 3333-3333",
                            Type = PhoneType.Work
                        }
                    }
                };

                _context.Employees.Add(adminEmployee);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário administrador criado com sucesso!");
                _logger.LogInformation("Email: admin@inbev.com | Senha: Admin@123");
            }
            else
            {
                _logger.LogInformation("Usuário administrador já existe no banco de dados.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar o seed de dados");
            throw;
        }
    }
}

