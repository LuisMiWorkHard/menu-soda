namespace MenuSoda.Domain.Models.Repositories;

public class PlatoUpdateRequest
{
    public int Id { get; set; }
    public string Planom { get; set; } = "";
    public string Plades { get; set; } = "";
    public int Codtippla { get; set; }
    public int Codest { get; set; }
    public string Usumod { get; set; } = "";
}
