using System.Security.Claims;
using InBev.Application.DTOs;
using InBev.Domain.Entities;
using InBev.Domain.Enums;
using InBev.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InBev.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        ILogger<EmployeesController> logger)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os funcionários com paginação e busca
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <param name="searchTerm">Termo de busca para filtrar por nome, email ou CPF</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        // Validar parâmetros
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var (employees, totalCount) = await _employeeRepository.GetPagedAsync(pageNumber, pageSize, searchTerm);

        var employeeDtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            DocNumber = e.DocNumber,
            BirthDate = e.BirthDate,
            Role = e.Role,
            ManagerId = e.ManagerId,
            ManagerName = e.Manager != null ? $"{e.Manager.FirstName} {e.Manager.LastName}" : null,
            PhoneNumbers = e.PhoneNumbers.Select(p => new PhoneNumberDto
            {
                Id = p.Id,
                Number = p.Number,
                Type = p.Type
            }).ToList()
        }).ToList();

        var response = new PagedResponse<EmployeeDto>(employeeDtos, totalCount, pageNumber, pageSize);

        return Ok(response);
    }

    /// <summary>
    /// Retorna um funcionário por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        var employeeDto = new EmployeeDto
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
        };

        return Ok(employeeDto);
    }

    /// <summary>
    /// Cria um novo funcionário
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        _logger.LogInformation("Criando novo funcionário: {Email}", dto.Email);

        // Obter o usuário atual
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (currentUserRole == null)
        {
            return Forbid();
        }

        var currentRole = Enum.Parse<EmployeeRole>(currentUserRole);

        // Regra: Não pode criar usuário com permissões maiores que a sua
        if (dto.Role > currentRole)
        {
            _logger.LogWarning("Tentativa de criar funcionário com permissão maior. Usuário atual: {CurrentRole}, Tentativa: {NewRole}", 
                currentRole, dto.Role);
            return Forbid("Você não pode criar um usuário com permissões maiores que a sua");
        }

        // Verificar se email já existe
        var existingEmail = await _employeeRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            return BadRequest(new { message = "Email já cadastrado" });
        }

        // Verificar se CPF já existe
        var existingDoc = await _employeeRepository.GetByDocNumberAsync(dto.DocNumber);
        if (existingDoc != null)
        {
            return BadRequest(new { message = "CPF já cadastrado" });
        }

        // Verificar se o gerente existe (se fornecido)
        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _employeeRepository.ExistsAsync(dto.ManagerId.Value);
            if (!managerExists)
            {
                return BadRequest(new { message = "Gerente não encontrado" });
            }
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DocNumber = dto.DocNumber,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            BirthDate = dto.BirthDate,
            Role = dto.Role,
            ManagerId = dto.ManagerId,
            PhoneNumbers = dto.PhoneNumbers.Select(p => new PhoneNumber
            {
                Id = Guid.NewGuid(),
                Number = p.Number,
                Type = p.Type
            }).ToList()
        };

        await _employeeRepository.AddAsync(employee);

        _logger.LogInformation("Funcionário criado com sucesso: {EmployeeId}", employee.Id);

        var employeeDto = new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DocNumber = employee.DocNumber,
            BirthDate = employee.BirthDate,
            Role = employee.Role,
            ManagerId = employee.ManagerId,
            PhoneNumbers = employee.PhoneNumbers.Select(p => new PhoneNumberDto
            {
                Id = p.Id,
                Number = p.Number,
                Type = p.Type
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employeeDto);
    }

    /// <summary>
    /// Atualiza um funcionário existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        _logger.LogInformation("Atualizando funcionário: {EmployeeId}", id);

        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        // Verificar se o gerente existe (se fornecido)
        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _employeeRepository.ExistsAsync(dto.ManagerId.Value);
            if (!managerExists)
            {
                return BadRequest(new { message = "Gerente não encontrado" });
            }
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.BirthDate = dto.BirthDate;
        employee.ManagerId = dto.ManagerId;

        // Atualizar telefones
        employee.PhoneNumbers.Clear();
        employee.PhoneNumbers = dto.PhoneNumbers.Select(p => new PhoneNumber
        {
            Id = Guid.NewGuid(),
            Number = p.Number,
            Type = p.Type,
            EmployeeId = employee.Id
        }).ToList();

        await _employeeRepository.UpdateAsync(employee);

        _logger.LogInformation("Funcionário atualizado com sucesso: {EmployeeId}", id);

        var employeeDto = new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DocNumber = employee.DocNumber,
            BirthDate = employee.BirthDate,
            Role = employee.Role,
            ManagerId = employee.ManagerId,
            PhoneNumbers = employee.PhoneNumbers.Select(p => new PhoneNumberDto
            {
                Id = p.Id,
                Number = p.Number,
                Type = p.Type
            }).ToList()
        };

        return Ok(employeeDto);
    }

    /// <summary>
    /// Remove (soft delete) um funcionário
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Removendo funcionário: {EmployeeId}", id);

        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        await _employeeRepository.DeleteAsync(id);

        _logger.LogInformation("Funcionário removido com sucesso: {EmployeeId}", id);

        return NoContent();
    }

    /// <summary>
    /// Retorna os subordinados de um gerente
    /// </summary>
    [HttpGet("{id:guid}/subordinates")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubordinates(Guid id)
    {
        var subordinates = await _employeeRepository.GetByManagerIdAsync(id);

        var employeeDtos = subordinates.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            DocNumber = e.DocNumber,
            BirthDate = e.BirthDate,
            Role = e.Role,
            ManagerId = e.ManagerId,
            PhoneNumbers = e.PhoneNumbers.Select(p => new PhoneNumberDto
            {
                Id = p.Id,
                Number = p.Number,
                Type = p.Type
            }).ToList()
        });

        return Ok(employeeDtos);
    }
}

