using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators
{
    public class EntradaCreateRequestValidator : AbstractValidator<EntradaCreateRequest>
    {

        public EntradaCreateRequestValidator()
        {

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción de la entrada es requerida.")
                .MinimumLength(3).WithMessage("La descripción debe tener al menos 3 caracteres.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

            RuleFor(x => x.DescripcionLarga)
                .MaximumLength(1000).WithMessage("La descripción larga no puede exceder los 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.DescripcionLarga));

            RuleFor(x => x.TipoEntradaId)
                .NotEmpty().WithMessage("El tipo de entrada es requerido.")
                .GreaterThan(0).WithMessage("El tipo de entrada debe ser un número entero válido.");

            RuleFor(x => x.ImagenId)
                .GreaterThan(0).WithMessage("El código de imagen debe ser un número válido.")
                .When(x => x.ImagenId.HasValue);
        }
    }
}
