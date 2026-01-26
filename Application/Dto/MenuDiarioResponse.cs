namespace MenuSoda.Application.Dto;

public class MenuDiarioResponse
{
    public int Id { get; set; }
    public string Mendiafec { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Fecreg { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }

    public List<dynamic> Entradas { get; set; } = new();
    public List<dynamic> Platos { get; set; } = new();
    public dynamic? Imagen { get; set; }
}
