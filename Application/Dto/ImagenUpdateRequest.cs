namespace MenuSoda.Application.Dto;

public class ImagenUpdateRequest
{
    public int Id { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
