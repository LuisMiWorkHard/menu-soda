using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class PlatoCreateRequestValidator : AbstractValidator<PlatoCreateRequest>
{
    public PlatoCreateRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del plato es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del plato no puede exceder los 100 caracteres.");
            
        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción del plato es obligatoria.")
            .MaximumLength(150).WithMessage("La descripción del plato no puede exceder los 150 caracteres.");

        RuleFor(x => x.TipoPlatoId)
            .GreaterThan(0).WithMessage("El ID del tipo de plato debe ser mayor a 0.");

        RuleFor(x => x.ImagenId)
            .GreaterThan(0).When(x => x.ImagenId.HasValue)
            .WithMessage("El ID de imagen debe ser mayor a 0.");
    }
}
