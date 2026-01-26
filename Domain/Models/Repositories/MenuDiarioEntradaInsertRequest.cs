namespace MenuSoda.Domain.Models.Repositories;

public class MenuDiarioEntradaInsertRequest
{
    public int Codmendia { get; set; }
    public int Codent { get; set; }
    public string Usureg { get; set; } = string.Empty;
}
