namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de adicional asociado a un plato del menú diario
/// </summary>
public class MenuDiarioPlatoAdicionalResponse
{
    public int Id { get; set; }
    public int MenuDiarioPlatoId { get; set; }
    public int AdicionalId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int EstadoId { get; set; }
}
