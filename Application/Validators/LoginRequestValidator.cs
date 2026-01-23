using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.TipoDocumento)
                .NotEmpty().WithMessage("El tipo de documento es requerido.")
                .InclusiveBetween(1, 2).WithMessage("El tipo de documento debe ser 1 (DNI) o 2 (CE).");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty().WithMessage("El número de documento es requerido.")
                .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.NumeroDocumento)
                        .Must((req, num) => req.TipoDocumento != 1 || System.Text.RegularExpressions.Regex.IsMatch(num, @"^\d{8}$"))
                        .WithMessage("Para el tipo de documento 1, el número debe ser de 8 dígitos numéricos.")
                        .Must((req, num) => req.TipoDocumento != 2 || System.Text.RegularExpressions.Regex.IsMatch(num, @"^[a-zA-Z0-9]{12}$"))
                        .WithMessage("Para el tipo de documento 2, el número debe ser alfanumérico de 12 caracteres.");
                });

            RuleFor(x => x.Contrasena)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(255).WithMessage("La contraseña no puede exceder los 255 caracteres.");
        }
    }
}
