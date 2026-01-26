namespace MenuSoda.Application.Dto;

public class TipoEntradaCreateServiceRequest
{
    public string Tipentdes { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
}

public class TipoEntradaUpdateServiceRequest
{
    public int Id { get; set; }
    public string Tipentdes { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
