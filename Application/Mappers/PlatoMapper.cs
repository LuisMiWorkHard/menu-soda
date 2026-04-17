using Riok.Mapperly.Abstractions;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Mappers;

[Mapper]
public static partial class PlatoMapper
{
    [MapProperty("Planom", "Nombre")]
    [MapProperty("Plades", "Descripcion")]
    [MapProperty("Codtippla", "TipoPlatoId")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Codima", "ImagenId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial PlatoResponse Map(Plato entity);
}
