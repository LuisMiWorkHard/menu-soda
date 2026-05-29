using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class VerificarCodigoRecuperacionRequestValidator : AbstractValidator<VerificarCodigoRecuperacionRequest>
{
    public VerificarCodigoRecuperacionRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .Length(4).WithMessage("El código debe tener exactamente 4 dígitos.")
            .Matches("^[0-9]{4}$").WithMessage("El código solo puede contener dígitos numéricos.");
    }
}
