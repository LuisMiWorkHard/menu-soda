namespace MenuSoda.Application.Dto;

public class MenuDiarioPlatoInsertRequest
{
    public int Codmendia { get; set; }
    public int Codpla { get; set; }
    public string Usureg { get; set; } = string.Empty;
    public int Orden { get; set; }
}
