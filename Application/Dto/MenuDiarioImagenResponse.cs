namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de imagen asociada a un menú diario
/// </summary>
public class MenuDiarioImagenResponse
{
    public int Id { get; set; }
    public int ImagenId { get; set; }
    public int? MenuImagenId { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
