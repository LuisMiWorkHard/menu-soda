namespace MenuSoda.Domain.Models.Repositories;

public class TipoPlatoUpdateRequest
{
    public int Id { get; set; }
    public string Tipplades { get; set; } = "";
    public int Codest { get; set; }
    public string Usumod { get; set; } = "";
}
