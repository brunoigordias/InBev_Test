using InBev.Domain.Entities;
using InBev.Domain.Interfaces;
using InBev.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InBev.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.PhoneNumbers)
            .Include(e => e.Manager)
            .Include(e => e.Subordinates)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .Include(e => e.PhoneNumbers)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Employee?> GetByDocNumberAsync(string docNumber)
    {
        return await _context.Employees
            .Include(e => e.PhoneNumbers)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.DocNumber == docNumber);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.PhoneNumbers)
            .Include(e => e.Manager)
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByManagerIdAsync(Guid managerId)
    {
        return await _context.Employees
            .Include(e => e.PhoneNumbers)
            .Where(e => e.ManagerId == managerId && e.IsActive)
            .ToListAsync();
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        employee.CreatedAt = DateTime.UtcNow;
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            employee.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id && e.IsActive);
    }
}

