namespace MenuSoda.Domain.Models.Repositories;

/// <summary>
/// Respuesta de imagen asociada a un menú diario
/// </summary>
public class MenuDiarioImagenResponse
{
    public int Id { get; set; }
    public int Codima { get; set; }
    public string Imarut { get; set; } = string.Empty;
    public string Imanom { get; set; } = string.Empty;
    public string Imaext { get; set; } = string.Empty;
    public int Codest { get; set; }
}
