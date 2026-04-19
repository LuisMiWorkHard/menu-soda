using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class MenuImagenCreateRequestValidator : AbstractValidator<MenuImagenCreateRequest>
{
    public MenuImagenCreateRequestValidator()
    {
        RuleFor(x => x.ImagenId)
            .GreaterThan(0).WithMessage("Debe indicar una imagen válida.");
    }
}
