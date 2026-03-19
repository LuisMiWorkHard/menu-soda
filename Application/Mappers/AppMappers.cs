using Riok.Mapperly.Abstractions;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Mappers;

[Mapper]
public static partial class AppMappers
{
    [MapProperty("Planom", "Nombre")]
    [MapProperty("Plades", "Descripcion")]
    [MapProperty("Codtippla", "TipoPlatoId")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial PlatoResponse Map(Plato entity);

    [MapProperty("Entdes", "Descripcion")]
    [MapProperty("Entdeslar", "DescripcionLarga")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Codtipent", "TipoEntradaId")]
    [MapProperty("Codima", "ImagenId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial EntradaResponse Map(Entrada entity);

    [MapProperty("Tipplades", "Descripcion")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial TipoPlatoResponse Map(TipoPlato entity);

    [MapProperty("Tipentdes", "Descripcion")]
    [MapProperty("Codest", "EstadoId")]
    [MapProperty("Fecreg", "FechaRegistro")]
    [MapProperty("Usureg", "UsuarioRegistro")]
    [MapProperty("Fecmod", "FechaModificacion")]
    [MapProperty("Usumod", "UsuarioModificacion")]
    public static partial TipoEntradaResponse Map(TipoEntrada entity);
}
