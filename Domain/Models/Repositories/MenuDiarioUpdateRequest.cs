namespace MenuSoda.Domain.Models.Repositories;

public class MenuDiarioUpdateRequest
{
    public int Id { get; set; }
    public DateTime Mendiafec { get; set; }
    public int Codest { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
