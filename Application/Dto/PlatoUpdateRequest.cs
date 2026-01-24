namespace MenuSoda.Application.Dto;

public class PlatoUpdateRequest
{
    public int Id { get; set; }
    public string Planom { get; set; } = string.Empty;
    public string Plades { get; set; } = string.Empty;
    public int Codtippla { get; set; }
    public int Codest { get; set; }
}
