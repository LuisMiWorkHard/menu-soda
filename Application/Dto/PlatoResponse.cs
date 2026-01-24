namespace MenuSoda.Application.Dto;

public class PlatoResponse
{
    public int Id { get; set; }
    public string Planom { get; set; } = "";
    public string Plades { get; set; } = "";
    public int Codtippla { get; set; }
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
