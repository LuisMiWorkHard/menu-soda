namespace MenuSoda.Application.Dto;

public class TipoPlatoCreateServiceRequest
{
    public string Tipplades { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
}

public class TipoPlatoUpdateServiceRequest
{
    public int Id { get; set; }
    public string Tipplades { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
