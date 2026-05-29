using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class RestablecerContrasenaRecuperacionRequestValidator : AbstractValidator<RestablecerContrasenaRecuperacionRequest>
{
    public RestablecerContrasenaRecuperacionRequestValidator()
    {
        RuleFor(x => x.NuevaContrasena)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(255).WithMessage("La contraseña no puede exceder los 255 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial.");

        RuleFor(x => x.ConfirmarContrasena)
            .NotEmpty().WithMessage("La confirmación de contraseña es requerida.");
    }
}
