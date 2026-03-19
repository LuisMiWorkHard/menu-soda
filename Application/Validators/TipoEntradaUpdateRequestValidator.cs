using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class TipoEntradaUpdateRequestValidator : AbstractValidator<TipoEntradaUpdateRequest>
{
    public TipoEntradaUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

        RuleFor(x => x.EstadoId)
            .InclusiveBetween(0, 1).WithMessage("El estado debe ser 0 (inactivo) o 1 (activo).");
    }
}
