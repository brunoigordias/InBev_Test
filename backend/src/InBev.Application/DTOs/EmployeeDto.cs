using InBev.Domain.Enums;

namespace InBev.Application.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public EmployeeRole Role { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
}

public class PhoneNumberDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; }
}

public class CreateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public EmployeeRole Role { get; set; }
    public Guid? ManagerId { get; set; }
    public List<CreatePhoneNumberDto> PhoneNumbers { get; set; } = new();
}

public class CreatePhoneNumberDto
{
    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; }
}

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Guid? ManagerId { get; set; }
    public List<CreatePhoneNumberDto> PhoneNumbers { get; set; } = new();
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public EmployeeDto Employee { get; set; } = null!;
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

