using FluentValidation;
using InBev.Application.DTOs;
using InBev.Domain.Enums;

namespace InBev.Application.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório")
            .MaximumLength(100).WithMessage("O sobrenome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("O email deve ter no máximo 200 caracteres");

        RuleFor(x => x.DocNumber)
            .NotEmpty().WithMessage("O CPF é obrigatório")
            .Must(BeValidCpf).WithMessage("CPF inválido")
            .MaximumLength(14).WithMessage("O CPF deve ter no máximo 14 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória")
            .Must(BeAtLeast18YearsOld).WithMessage("O funcionário deve ser maior de idade (18 anos ou mais)");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Função inválida");

        // Validação de ManagerId baseado no Role
        RuleFor(x => x.ManagerId)
            .Null()
            .When(x => x.Role == EmployeeRole.Manager)
            .WithMessage("Gerentes não podem ter gerente");

        RuleFor(x => x.ManagerId)
            .NotNull()
            .When(x => x.Role == EmployeeRole.Employee)
            .WithMessage("Funcionários devem ter um gerente");

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty().WithMessage("Deve haver pelo menos um telefone")
            .Must(x => x.Count > 0).WithMessage("Deve haver pelo menos um telefone");

        RuleForEach(x => x.PhoneNumbers).SetValidator(new CreatePhoneNumberValidator());
    }

    private bool BeValidCpf(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // CPF deve ter 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Validação do primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        // Validação do segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }

    private bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }
}

public class CreatePhoneNumberValidator : AbstractValidator<CreatePhoneNumberDto>
{
    public CreatePhoneNumberValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("O número de telefone é obrigatório")
            .Matches(@"^\(?[1-9]{2}\)?\s?9?\d{4}-?\d{4}$")
            .WithMessage("Número de telefone inválido");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de telefone inválido");
    }
}

