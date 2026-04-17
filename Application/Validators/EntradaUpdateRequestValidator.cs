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

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la entrada es requerido.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.TipoEntradaId)
                .NotEmpty().WithMessage("El tipo de entrada es requerido.")
                .GreaterThan(0).WithMessage("El tipo de entrada debe ser un número entero válido.");

            RuleFor(x => x.EstadoId)
                .InclusiveBetween(0, 1).WithMessage("El código de estado debe ser 0 (inactivo) o 1 (activo).");
        }
    }
}
