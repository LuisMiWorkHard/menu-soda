using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Mappers;

public static class ImagenMapper
{
    public static ImagenResponse ToResponse(this Imagen entity, IStorageService storageService)
    {
        return new ImagenResponse
        {
            Id = entity.Id,
            Ruta = storageService.GetSignedUrl(entity.Imarut),
            Nombre = entity.Imanom,
            Extension = entity.Imaext,
            EstadoId = entity.Codest,
            FechaRegistro = entity.Fecreg,
            UsuarioRegistro = entity.Usureg,
            FechaModificacion = entity.Fecmod,
            UsuarioModificacion = entity.Usumod
        };
    }
}
