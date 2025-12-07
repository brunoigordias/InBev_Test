using FluentAssertions;
using InBev.Domain.Entities;
using InBev.Domain.Enums;
using InBev.Infrastructure.Data;
using InBev.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InBev.UnitTests.Infrastructure.Repositories;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        // Configurar banco em memória para testes
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmployeeRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Employee_To_Database()
    {
        // Arrange
        var employee = CreateTestEmployee();

        // Act
        var result = await _repository.AddAsync(employee);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employee.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var savedEmployee = await _context.Employees.FindAsync(employee.Id);
        savedEmployee.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Employee_When_Exists()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(employee.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employee.Id);
        result.Email.Should().Be(employee.Email);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Employee_When_Exists()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(employee.Email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(employee.Email);
    }

    [Fact]
    public async Task GetByDocNumberAsync_Should_Return_Employee_When_Exists()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDocNumberAsync(employee.DocNumber);

        // Assert
        result.Should().NotBeNull();
        result!.DocNumber.Should().Be(employee.DocNumber);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Only_Active_Employees()
    {
        // Arrange
        var activeEmployee = CreateTestEmployee();
        var inactiveEmployee = CreateTestEmployee();
        inactiveEmployee.IsActive = false;
        
        await _context.Employees.AddRangeAsync(activeEmployee, inactiveEmployee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(e => e.Id == activeEmployee.Id);
        result.Should().NotContain(e => e.Id == inactiveEmployee.Id);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Employee_And_Set_UpdatedAt()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        employee.FirstName = "Updated Name";

        // Act
        await _repository.UpdateAsync(employee);

        // Assert
        var updated = await _context.Employees.FindAsync(employee.Id);
        updated!.FirstName.Should().Be("Updated Name");
        updated.UpdatedAt.Should().NotBeNull();
        updated.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAsync_Should_Soft_Delete_Employee()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(employee.Id);

        // Assert
        var deleted = await _context.Employees.FindAsync(employee.Id);
        deleted!.IsActive.Should().BeFalse();
        deleted.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Active_Employee_Exists()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(employee.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_False_For_Inactive_Employee()
    {
        // Arrange
        var employee = CreateTestEmployee();
        employee.IsActive = false;
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(employee.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByManagerIdAsync_Should_Return_Only_Active_Subordinates()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var activeSubordinate = CreateTestEmployee();
        activeSubordinate.ManagerId = managerId;
        
        var inactiveSubordinate = CreateTestEmployee();
        inactiveSubordinate.ManagerId = managerId;
        inactiveSubordinate.IsActive = false;

        await _context.Employees.AddRangeAsync(activeSubordinate, inactiveSubordinate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByManagerIdAsync(managerId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(e => e.Id == activeSubordinate.Id);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Correct_Page_And_TotalCount()
    {
        // Arrange - Criar 15 funcionários
        var employees = new List<Employee>();
        for (int i = 0; i < 15; i++)
        {
            var employee = CreateTestEmployee();
            employee.FirstName = $"Employee{i:D2}"; // Employee00, Employee01, etc.
            employees.Add(employee);
        }
        await _context.Employees.AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        // Act - Pegar página 2 com 5 itens por página
        var (items, totalCount) = await _repository.GetPagedAsync(2, 5);

        // Assert
        totalCount.Should().Be(15);
        items.Should().HaveCount(5);
        items.First().FirstName.Should().Be("Employee05"); // Ordenado alfabeticamente
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Only_Active_Employees()
    {
        // Arrange
        var activeEmployee1 = CreateTestEmployee();
        activeEmployee1.FirstName = "Active1";
        
        var activeEmployee2 = CreateTestEmployee();
        activeEmployee2.FirstName = "Active2";
        
        var inactiveEmployee = CreateTestEmployee();
        inactiveEmployee.FirstName = "Inactive";
        inactiveEmployee.IsActive = false;

        await _context.Employees.AddRangeAsync(activeEmployee1, activeEmployee2, inactiveEmployee);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10);

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().NotContain(e => e.Id == inactiveEmployee.Id);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Filter_By_FirstName()
    {
        // Arrange
        var employee1 = CreateTestEmployee();
        employee1.FirstName = "Carlos";
        
        var employee2 = CreateTestEmployee();
        employee2.FirstName = "Maria";
        
        var employee3 = CreateTestEmployee();
        employee3.FirstName = "Carla";

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act - Buscar por "car" (deve encontrar Carlos e Carla)
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "car");

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().Contain(e => e.FirstName == "Carlos");
        items.Should().Contain(e => e.FirstName == "Carla");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Filter_By_LastName()
    {
        // Arrange
        var employee1 = CreateTestEmployee();
        employee1.LastName = "Santos";
        
        var employee2 = CreateTestEmployee();
        employee2.LastName = "Silva";
        
        var employee3 = CreateTestEmployee();
        employee3.LastName = "Santana";

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act - Buscar por "sant" (deve encontrar Santos e Santana)
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "sant");

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().Contain(e => e.LastName == "Santos");
        items.Should().Contain(e => e.LastName == "Santana");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Filter_By_Email()
    {
        // Arrange
        var employee1 = CreateTestEmployee();
        employee1.Email = "joao@empresa.com";
        
        var employee2 = CreateTestEmployee();
        employee2.Email = "maria@empresa.com";
        
        var employee3 = CreateTestEmployee();
        employee3.Email = "joao.silva@empresa.com";

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act - Buscar por "joao" (deve encontrar 2 emails)
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "joao");

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().Contain(e => e.Email == "joao@empresa.com");
        items.Should().Contain(e => e.Email == "joao.silva@empresa.com");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Filter_By_DocNumber()
    {
        // Arrange
        var employee1 = CreateTestEmployee();
        employee1.DocNumber = "12345678901";
        
        var employee2 = CreateTestEmployee();
        employee2.DocNumber = "98765432109";
        
        var employee3 = CreateTestEmployee();
        employee3.DocNumber = "12398765432";

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act - Buscar por "123" (deve encontrar 2 CPFs)
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "123");

        // Assert
        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
        items.Should().Contain(e => e.DocNumber == "12345678901");
        items.Should().Contain(e => e.DocNumber == "12398765432");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Be_Case_Insensitive()
    {
        // Arrange
        var employee = CreateTestEmployee();
        employee.FirstName = "Carlos";
        employee.Email = "carlos@empresa.com";

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act - Buscar com diferentes cases
        var (itemsUpper, totalCountUpper) = await _repository.GetPagedAsync(1, 10, "CARLOS");
        var (itemsLower, totalCountLower) = await _repository.GetPagedAsync(1, 10, "carlos");
        var (itemsMixed, totalCountMixed) = await _repository.GetPagedAsync(1, 10, "CaRlOs");

        // Assert
        totalCountUpper.Should().Be(1);
        totalCountLower.Should().Be(1);
        totalCountMixed.Should().Be(1);
        itemsUpper.Should().HaveCount(1);
        itemsLower.Should().HaveCount(1);
        itemsMixed.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Empty_When_No_Results()
    {
        // Arrange
        var employee = CreateTestEmployee();
        employee.FirstName = "João";
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act - Buscar algo que não existe
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "XYZ123NotFound");

        // Assert
        totalCount.Should().Be(0);
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Empty_When_Page_Exceeds_Total()
    {
        // Arrange
        var employee = CreateTestEmployee();
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act - Pedir página 5 quando só existe 1 funcionário
        var (items, totalCount) = await _repository.GetPagedAsync(5, 10);

        // Assert
        totalCount.Should().Be(1);
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPagedAsync_Should_Order_By_FirstName_Then_LastName()
    {
        // Arrange
        var employee1 = CreateTestEmployee();
        employee1.FirstName = "Bruno";
        employee1.LastName = "Zeta";

        var employee2 = CreateTestEmployee();
        employee2.FirstName = "Ana";
        employee2.LastName = "Silva";

        var employee3 = CreateTestEmployee();
        employee3.FirstName = "Bruno";
        employee3.LastName = "Alpha";

        await _context.Employees.AddRangeAsync(employee1, employee2, employee3);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10);

        // Assert
        var itemsList = items.ToList();
        itemsList[0].FirstName.Should().Be("Ana");
        itemsList[1].FirstName.Should().Be("Bruno");
        itemsList[1].LastName.Should().Be("Alpha");
        itemsList[2].FirstName.Should().Be("Bruno");
        itemsList[2].LastName.Should().Be("Zeta");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Include_Manager_And_PhoneNumbers()
    {
        // Arrange
        var manager = CreateTestEmployee();
        manager.FirstName = "Manager";
        await _context.Employees.AddAsync(manager);
        await _context.SaveChangesAsync();

        var employee = CreateTestEmployee();
        employee.ManagerId = manager.Id;
        employee.PhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber { Id = Guid.NewGuid(), Number = "11999999999", Type = PhoneType.Mobile }
        };
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10);

        // Assert
        var result = items.FirstOrDefault(e => e.Id == employee.Id);
        result.Should().NotBeNull();
        result!.Manager.Should().NotBeNull();
        result.Manager!.FirstName.Should().Be("Manager");
        result.PhoneNumbers.Should().HaveCount(1);
        result.PhoneNumbers.First().Number.Should().Be("11999999999");
    }

    private Employee CreateTestEmployee()
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "João",
            LastName = "Silva",
            Email = $"joao.{Guid.NewGuid()}@example.com",
            DocNumber = Guid.NewGuid().ToString().Substring(0, 11),
            PasswordHash = "hashedpassword",
            BirthDate = DateTime.Today.AddYears(-25),
            Role = EmployeeRole.Employee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

