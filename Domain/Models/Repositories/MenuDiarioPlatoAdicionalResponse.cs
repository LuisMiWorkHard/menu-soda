namespace MenuSoda.Domain.Models.Repositories;

/// <summary>
/// Respuesta de adicional asociado a un plato del menú diario
/// </summary>
public class MenuDiarioPlatoAdicionalResponse
{
    public int Id { get; set; }
    public int Codmendiapla { get; set; }
    public int Codadi { get; set; }
    public string Adinom { get; set; } = string.Empty;
    public string? Adides { get; set; }
    public int Codest { get; set; }
}
