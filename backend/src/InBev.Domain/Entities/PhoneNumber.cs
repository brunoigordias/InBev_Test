using InBev.Domain.Enums;

namespace InBev.Domain.Entities;

public class PhoneNumber
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; }
    
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}

