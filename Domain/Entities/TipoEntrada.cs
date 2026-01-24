namespace MenuSoda.Domain.Entities;

public class TipoEntrada
{
    public int Id { get; set; }
    public string Tipentdes { get; set; } = "";
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
