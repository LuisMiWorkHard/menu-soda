namespace MenuSoda.Domain.Models.Repositories;

public class MenuDiarioPlatoAdicionalInsertRequest
{
    public int Codmendiapla { get; set; }
    public int Codadi { get; set; }
    public string Usureg { get; set; } = string.Empty;
}
