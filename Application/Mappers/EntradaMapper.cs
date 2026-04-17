using Riok.Mapperly.Abstractions;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Mappers;

[Mapper]
public static partial class EntradaMapper
{
    [MapProperty("Entnom", "Nombre")]
    [MapProperty("Entdes", "Descripcion")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Codtipent", "TipoEntradaId")]
    [MapProperty("Codima", "ImagenId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial EntradaResponse Map(Entrada entity);
}
