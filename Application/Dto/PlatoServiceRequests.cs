namespace MenuSoda.Application.Dto;

public class PlatoCreateServiceRequest
{
    public string Planom { get; set; } = string.Empty;
    public string Plades { get; set; } = string.Empty;
    public int Codtippla { get; set; }
    public string Usureg { get; set; } = string.Empty;
}

public class PlatoUpdateServiceRequest
{
    public int Id { get; set; }
    public string Planom { get; set; } = string.Empty;
    public string Plades { get; set; } = string.Empty;
    public int Codtippla { get; set; }
    public int Codest { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
