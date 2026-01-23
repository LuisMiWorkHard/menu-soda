namespace MenuSoda.Domain.Models.Repositories;

public class EntradaUpdateRequest
{
    public int Id { get; set; }
    public string Entdes { get; set; } = "";
    public int Codest { get; set; }
    public string Usumod { get; set; } = "";
}
