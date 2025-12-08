using FluentValidation;
using InBev.Application.DTOs;

namespace InBev.Application.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Nova senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("Nova senha deve ter no mínimo 6 caracteres");

        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .When(x => !string.IsNullOrEmpty(x.CurrentPassword) && !string.IsNullOrEmpty(x.NewPassword))
            .WithMessage("Nova senha deve ser diferente da senha atual");
    }
}

