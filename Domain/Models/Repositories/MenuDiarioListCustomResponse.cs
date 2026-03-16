namespace MenuSoda.Domain.Models.Repositories;

/// <summary>
/// Respuesta del stored procedure sp_get_menu_diario_list_custom
/// Incluye datos agregados del menú diario con conteos y detalles
/// Todas las fechas vienen en formato DD/MM/YYYY o DD/MM/YYYY HH:MM desde la BD
/// </summary>
public class MenuDiarioListCustomResponse
{
    public int Id { get; set; }
    public string Mendiafec { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Fecreg { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }

    // Campos adicionales del reporte
    public int? Cantidad_entradas { get; set; }
    public string? Platos_por_tipo { get; set; } // JSON string desde PostgreSQL
}
