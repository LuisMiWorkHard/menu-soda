using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class TipoPlatoCreateRequestValidator : AbstractValidator<TipoPlatoCreateRequest>
{
    public TipoPlatoCreateRequestValidator()
    {
        RuleFor(x => x.Tipplades)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");
    }
}