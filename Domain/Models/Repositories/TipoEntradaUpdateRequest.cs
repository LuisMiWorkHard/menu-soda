namespace MenuSoda.Domain.Models.Repositories;

public class TipoEntradaUpdateRequest
{
    public int Id { get; set; }
    public string Tipentdes { get; set; } = "";
    public int Codest { get; set; }
    public string Usumod { get; set; } = "";
}
