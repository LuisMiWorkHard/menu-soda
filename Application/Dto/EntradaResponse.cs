namespace MenuSoda.Application.Dto;

public class EntradaResponse
{
    public int Id { get; set; }
    public string Entdes { get; set; } = "";
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
