using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.TipoDocumento)
            .GreaterThan(0)
            .WithMessage("El tipo de documento es requerido.");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty()
            .WithMessage("El número de documento es requerido.")
            .MaximumLength(20)
            .WithMessage("El número de documento no puede exceder 20 caracteres.")
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("El número de documento solo puede contener letras y números.");

        RuleFor(x => x.Contrasena)
            .NotEmpty()
            .WithMessage("La contraseña es requerida.")
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
