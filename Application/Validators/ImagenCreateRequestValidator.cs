using FluentValidation;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Validators;

public class ImagenCreateRequestValidator : AbstractValidator<ImagenCreateRequest>
{
    public ImagenCreateRequestValidator()
    {
        RuleFor(x => x.Ruta)
            .NotEmpty().WithMessage("La ruta de la imagen es obligatoria.")
            .MaximumLength(200).WithMessage("La ruta no puede exceder los 200 caracteres."); // Ajustar longitud según DB si es necesario

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la imagen es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Extension)
            .NotEmpty().WithMessage("La extensión es obligatoria.")
            .MaximumLength(10).WithMessage("La extensión no puede exceder los 10 caracteres.");
    }
}
