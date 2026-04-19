using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class MenuImagenUpdateRequestValidator : AbstractValidator<MenuImagenUpdateRequest>
{
    public MenuImagenUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0.");

        RuleFor(x => x.ImagenId)
            .GreaterThan(0).WithMessage("Debe indicar una imagen válida.");

        RuleFor(x => x.EstadoId)
            .InclusiveBetween(0, 1).WithMessage("El estado debe ser 0 (inactivo) o 1 (activo).");
    }
}
