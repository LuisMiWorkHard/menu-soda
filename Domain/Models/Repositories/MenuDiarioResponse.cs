namespace MenuSoda.Domain.Models.Repositories;

/// <summary>
/// Respuesta del menú diario desde stored procedures
/// Todas las fechas vienen en formato DD/MM/YYYY o DD/MM/YYYY HH:MM desde la BD
/// </summary>
public class MenuDiarioResponse
{
    public int Id { get; set; }
    public string Mendiafec { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Fecreg { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
