using InBev.Domain.Enums;

namespace InBev.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty; // CPF
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public EmployeeRole Role { get; set; }
    
    // Manager (gerente pode ser um funcionário)
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    
    // Telefones (deve ter mais de um)
    public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    
    // Subordinados
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

