namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de entrada asociada a un menú diario
/// </summary>
public class MenuDiarioEntradaResponse
{
    public int Id { get; set; }
    public int EntradaId { get; set; }
    public string EntradaNombre { get; set; } = string.Empty;
    public string? EntradaDescripcion { get; set; }
    public int TipoEntradaId { get; set; }
    public string TipoEntradaDescripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
