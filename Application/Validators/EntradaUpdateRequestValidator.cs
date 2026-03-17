using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators
{
    public class EntradaUpdateRequestValidator : AbstractValidator<EntradaUpdateRequest>
    {
        public EntradaUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID de la entrada debe ser mayor a cero.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción de la entrada es requerida.")
                .MinimumLength(3).WithMessage("La descripción debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.TipoEntradaId)
                .NotEmpty().WithMessage("El tipo de entrada es requerido.")
                .GreaterThan(0).WithMessage("El tipo de entrada debe ser un número entero válido.");

            RuleFor(x => x.EstadoId)
                .InclusiveBetween(0, 1).WithMessage("El código de estado debe ser 0 (inactivo) o 1 (activo).");
        }
    }
}
