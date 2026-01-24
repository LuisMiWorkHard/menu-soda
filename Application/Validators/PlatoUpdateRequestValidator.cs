using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class PlatoUpdateRequestValidator : AbstractValidator<PlatoUpdateRequest>
{
    public PlatoUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0.");

        RuleFor(x => x.Planom)
            .NotEmpty().WithMessage("El nombre del plato es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del plato no puede exceder los 100 caracteres.");

        RuleFor(x => x.Plades)
            .NotEmpty().WithMessage("La descripción del plato es obligatoria.")
            .MaximumLength(150).WithMessage("La descripción del plato no puede exceder los 150 caracteres.");

        RuleFor(x => x.Codtippla)
            .GreaterThan(0).WithMessage("El ID del tipo de plato debe ser mayor a 0.");

        RuleFor(x => x.Codest)
            .InclusiveBetween(0, 1).WithMessage("El estado debe ser 0 (inactivo) o 1 (activo).");
    }
}
