namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de entrada asociada a un menú diario
/// </summary>
public class MenuDiarioEntradaResponse
{
    public int Id { get; set; }
    public int Codent { get; set; }
    public string Entdes { get; set; } = string.Empty;
    public string? Entdeslar { get; set; }
    public int Codtipent { get; set; }
    public string Tipentdes { get; set; } = string.Empty;
    public int Codest { get; set; }
}
