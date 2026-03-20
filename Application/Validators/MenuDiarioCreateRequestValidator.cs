using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class MenuDiarioCreateRequestValidator : AbstractValidator<MenuDiarioCreateRequest>
{
    public MenuDiarioCreateRequestValidator()
    {
        RuleFor(x => x.EntradasIds)
            .NotEmpty().WithMessage("Se debe enviar al menos una entrada.");

        RuleFor(x => x.Platos)
            .NotEmpty().WithMessage("Se debe enviar al menos un plato.");
    }
}
