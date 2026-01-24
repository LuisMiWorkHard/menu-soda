using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators
{
    public class EntradaCreateRequestValidator : AbstractValidator<EntradaCreateRequest>
    {

        public EntradaCreateRequestValidator()
        {

            RuleFor(x => x.Entdes)
                .NotEmpty().WithMessage("La descripción de la entrada es requerida.")
                .MinimumLength(3).WithMessage("La descripción debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.Codtipent)
                .NotEmpty().WithMessage("El tipo de entrada es requerido.")
                .GreaterThan(0).WithMessage("El tipo de entrada debe ser un número entero válido.");
        }
    }
}
