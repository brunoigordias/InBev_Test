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

