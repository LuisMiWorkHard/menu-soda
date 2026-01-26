namespace MenuSoda.Domain.Entities;

public class MenuDiario
{
    public int Id { get; set; }
    public DateTime Mendiafec { get; set; }
    public int Codest { get; set; }
    public DateTime Fecreg { get; set; }
    public string Usureg { get; set; } = string.Empty;
    public DateTime? Fecmod { get; set; }
    public string? Usumod { get; set; }
}

public class MenuDiarioEntrada
{
    public int Id { get; set; }
    public int Codmendia { get; set; }
    public int Codent { get; set; }
    public int Codest { get; set; }
}

public class MenuDiarioPlato
{
    public int Id { get; set; }
    public int Codmendia { get; set; }
    public int Codpla { get; set; }
    public int Codest { get; set; }
}

public class MenuDiarioImagen
{
    public int Id { get; set; }
    public int Codmendia { get; set; }
    public int Codima { get; set; }
    public int Codest { get; set; }
}
