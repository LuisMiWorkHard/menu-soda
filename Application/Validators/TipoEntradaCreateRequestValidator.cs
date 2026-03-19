using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class TipoEntradaCreateRequestValidator : AbstractValidator<TipoEntradaCreateRequest>
{
    public TipoEntradaCreateRequestValidator()
    {
        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");
    }
}
