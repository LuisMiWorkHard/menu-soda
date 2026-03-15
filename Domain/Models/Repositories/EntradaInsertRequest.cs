namespace MenuSoda.Domain.Models.Repositories;

public class EntradaInsertRequest
{
    public string Entdes { get; set; } = "";
    public string? Entdeslar { get; set; }
    public int Codtipent { get; set; }
    public int? Codima { get; set; }
    public string Usureg { get; set; } = "";
}
