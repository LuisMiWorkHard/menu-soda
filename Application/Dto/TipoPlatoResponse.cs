using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Dto;

public class TipoPlatoResponse
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = "";
    public int EstadoId { get; set; }
    public string FechaRegistro { get; set; } = "";
    public string UsuarioRegistro { get; set; } = "";
    public string FechaModificacion { get; set; } = "";
    public string UsuarioModificacion { get; set; } = "";

    public static TipoPlatoResponse FromEntity(TipoPlato entity)
    {
        return new TipoPlatoResponse
        {
            Id = entity.Id,
            Descripcion = entity.Tipplades,
            EstadoId = entity.Codest,
            FechaRegistro = entity.Fecreg,
            UsuarioRegistro = entity.Usureg,
            FechaModificacion = entity.Fecmod,
            UsuarioModificacion = entity.Usumod
        };
    }
}
