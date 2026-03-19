using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class ImagenUpdateRequestValidator : AbstractValidator<ImagenUpdateRequest>
{
    public ImagenUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0.");

        RuleFor(x => x.Ruta)
            .NotEmpty().WithMessage("La ruta de la imagen es obligatoria.")
            .MaximumLength(200).WithMessage("La ruta no puede exceder los 200 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la imagen es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Extension)
            .NotEmpty().WithMessage("La extensión es obligatoria.")
            .MaximumLength(10).WithMessage("La extensión no puede exceder los 10 caracteres.");

        RuleFor(x => x.EstadoId)
            .InclusiveBetween(0, 1).WithMessage("El estado debe ser 0 (inactivo) o 1 (activo).");
    }
}
