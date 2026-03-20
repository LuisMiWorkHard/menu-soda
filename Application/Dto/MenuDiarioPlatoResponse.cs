namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta de plato asociado a un menú diario
/// </summary>
public class MenuDiarioPlatoResponse
{
    public int Id { get; set; }
    public int Codpla { get; set; }
    public string Planom { get; set; } = string.Empty;
    public string? Plades { get; set; }
    public int Codtippla { get; set; }
    public string Tipplades { get; set; } = string.Empty;
    public int Codest { get; set; }
}
