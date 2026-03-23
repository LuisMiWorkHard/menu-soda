using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class MenuDiarioUpdateRequestValidator : AbstractValidator<MenuDiarioUpdateRequest>
{
    public MenuDiarioUpdateRequestValidator()
    {
        RuleFor(x => x.EntradasIds)
            .NotEmpty().WithMessage("Se debe enviar al menos una entrada.");

        RuleFor(x => x.Platos)
            .NotEmpty().WithMessage("Se debe enviar al menos un plato.");
    }
}
