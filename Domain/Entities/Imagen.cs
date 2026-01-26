namespace MenuSoda.Domain.Entities;

public class Imagen
{
    public int Id { get; set; }
    public string Imarut { get; set; } = "";
    public string Imanom { get; set; } = "";
    public string Imaext { get; set; } = "";
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
}
