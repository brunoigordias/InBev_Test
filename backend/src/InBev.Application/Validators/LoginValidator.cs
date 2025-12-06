using FluentValidation;
using InBev.Application.DTOs;

namespace InBev.Application.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória");
    }
}

