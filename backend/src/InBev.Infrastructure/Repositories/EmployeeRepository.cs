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

    public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        var query = _context.Employees
            .Include(e => e.PhoneNumbers)
            .Include(e => e.Manager)
            .Where(e => e.IsActive);

        // Aplicar filtro de busca se fornecido
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(search) ||
                e.LastName.ToLower().Contains(search) ||
                e.Email.ToLower().Contains(search) ||
                e.DocNumber.Contains(search)
            );
        }

        // Obter contagem total
        var totalCount = await query.CountAsync();

        // Aplicar paginação
        var items = await query
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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
        
        // Buscar telefones existentes sem tracking para evitar conflito
        var existingPhones = await _context.PhoneNumbers
            .AsNoTracking()
            .Where(p => p.EmployeeId == employee.Id)
            .ToListAsync();
        
        // Remover telefones antigos se existirem
        if (existingPhones.Any())
        {
            // Precisamos anexar as entidades para deletá-las
            foreach (var phone in existingPhones)
            {
                _context.PhoneNumbers.Attach(phone);
                _context.PhoneNumbers.Remove(phone);
            }
        }
        
        // Marcar o employee como modificado
        _context.Entry(employee).State = EntityState.Modified;
        
        // Marcar os novos telefones como adicionados
        foreach (var phone in employee.PhoneNumbers)
        {
            _context.Entry(phone).State = EntityState.Added;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(Guid employeeId, string passwordHash)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee != null)
        {
            employee.PasswordHash = passwordHash;
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
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

