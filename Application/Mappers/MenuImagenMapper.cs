using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Mappers;

public static class MenuImagenMapper
{
    public static MenuImagenResponse ToResponse(this MenuImagen entity) =>
        new()
        {
            Id = entity.Id,
            ImagenId = entity.Codima,
            ImagenUrl = $"/api/menuimagen/{entity.Id}/contenido",
            ImagenRuta = entity.Imarut,
            ImagenExtension = Path.GetExtension(entity.Imarut),
            EstadoId = entity.Codest,
            FechaRegistro = entity.Fecreg,
            UsuarioRegistro = entity.Usureg,
            FechaModificacion = entity.Fecmod,
            UsuarioModificacion = entity.Usumod,
            Aretextop = entity.Aretextop,
            Aretexbot = entity.Aretexbot,
            Aretexini = entity.Aretexini,
            Aretexfin = entity.Aretexfin,
            Maxfonsiz = entity.Maxfonsiz,
            Fonfam    = entity.Fonfam
        };

    public static MenuImagenResponse ToResponse(this MenuImagen entity, IStorageService storageService) =>
        new()
        {
            Id = entity.Id,
            ImagenId = entity.Codima,
            ImagenUrl = storageService.GetSignedUrl(entity.Imarut),
            ImagenRuta = entity.Imarut,
            ImagenExtension = Path.GetExtension(entity.Imarut),
            EstadoId = entity.Codest,
            FechaRegistro = entity.Fecreg,
            UsuarioRegistro = entity.Usureg,
            FechaModificacion = entity.Fecmod,
            UsuarioModificacion = entity.Usumod,
            Aretextop = entity.Aretextop,
            Aretexbot = entity.Aretexbot,
            Aretexini = entity.Aretexini,
            Aretexfin = entity.Aretexfin,
            Maxfonsiz = entity.Maxfonsiz,
            Fonfam    = entity.Fonfam
        };
}
