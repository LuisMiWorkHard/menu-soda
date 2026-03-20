using Riok.Mapperly.Abstractions;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Mappers;

[Mapper]
public static partial class TipoPlatoMapper
{
    [MapProperty("Tipplades", "Descripcion")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial TipoPlatoResponse Map(TipoPlato entity);
}
