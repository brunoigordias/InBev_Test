using InBev.Application.DTOs;
using InBev.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InBev.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login de um funcionário
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Tentativa de login para o email: {Email}", loginDto.Email);

        var employee = await _employeeRepository.GetByEmailAsync(loginDto.Email);

        if (employee == null || !_passwordHasher.VerifyPassword(loginDto.Password, employee.PasswordHash))
        {
            _logger.LogWarning("Login falhou para o email: {Email}", loginDto.Email);
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }

        if (!employee.IsActive)
        {
            _logger.LogWarning("Tentativa de login de funcionário inativo: {Email}", loginDto.Email);
            return Unauthorized(new { message = "Funcionário inativo" });
        }

        var token = _tokenService.GenerateToken(employee);

        var response = new LoginResponseDto
        {
            Token = token,
            Employee = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DocNumber = employee.DocNumber,
                BirthDate = employee.BirthDate,
                Role = employee.Role,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager != null ? $"{employee.Manager.FirstName} {employee.Manager.LastName}" : null,
                PhoneNumbers = employee.PhoneNumbers.Select(p => new PhoneNumberDto
                {
                    Id = p.Id,
                    Number = p.Number,
                    Type = p.Type
                }).ToList()
            }
        };

        _logger.LogInformation("Login bem-sucedido para: {Email}", loginDto.Email);
        return Ok(response);
    }
}

