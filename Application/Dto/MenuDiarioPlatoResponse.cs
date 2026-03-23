namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de plato asociado a un menú diario
/// </summary>
public class MenuDiarioPlatoResponse
{
    public int Id { get; set; }
    public int PlatoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int TipoPlatoId { get; set; }
    public string TipoPlatoDescripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
