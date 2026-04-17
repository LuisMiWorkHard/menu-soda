namespace MenuSoda.Application.Dto;

public class PlatoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int TipoPlatoId { get; set; }
    public int EstadoId { get; set; }
    public int? ImagenId { get; set; }
    public string FechaRegistro { get; set; } = "";
    public string UsuarioRegistro { get; set; } = "";
    public string? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
}