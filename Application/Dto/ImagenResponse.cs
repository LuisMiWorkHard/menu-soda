namespace MenuSoda.Application.Dto;

public class ImagenResponse
{
    public int Id { get; set; }
    public string Ruta { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Extension { get; set; } = "";
    public int EstadoId { get; set; }
    public string FechaRegistro { get; set; } = "";
    public string UsuarioRegistro { get; set; } = "";
    public string? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
}
