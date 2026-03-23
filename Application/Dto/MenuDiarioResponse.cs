namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta del menú diario desde stored procedures
/// Todas las fechas vienen en formato DD/MM/YYYY o DD/MM/YYYY HH:MM desde la BD
/// </summary>
public class MenuDiarioResponse
{
    public int Id { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public int EstadoId { get; set; }
    public string FechaRegistro { get; set; } = string.Empty;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public string? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
}
