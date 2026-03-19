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


}
