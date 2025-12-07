using InBev.Domain.Entities;

namespace InBev.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByDocNumberAsync(string docNumber);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<IEnumerable<Employee>> GetByManagerIdAsync(Guid managerId);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

