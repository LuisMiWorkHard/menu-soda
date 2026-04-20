namespace MenuSoda.Domain.Entities;

public class MenuImagen
{
    public int Id { get; set; }
    public int Codima { get; set; }
    public string Imarut { get; set; } = "";
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
    public decimal Aretextop { get; set; } = 0.10m;
    public decimal Aretexbot { get; set; } = 0.10m;
    public decimal Aretexini { get; set; } = 0.08m;
    public decimal Aretexfin { get; set; } = 0.08m;
    public decimal Maxfonsiz { get; set; } = 17.00m;
    public string  Fonfam    { get; set; } = "default";
}
