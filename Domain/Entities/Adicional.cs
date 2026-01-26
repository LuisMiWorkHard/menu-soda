namespace MenuSoda.Domain.Entities;

public class Adicional
{
    public int Id { get; set; }
    public string Adides { get; set; } = string.Empty;
    public int Codest { get; set; }
    public DateTime Fecreg { get; set; }
    public string Usureg { get; set; } = string.Empty;
    public DateTime? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
