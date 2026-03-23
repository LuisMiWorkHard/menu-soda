namespace MenuSoda.Application.Dto;

public class MenuDiarioPlatoAdicionalInsertRequest
{
    public int MenuDiarioPlatoId { get; set; }
    public int AdicionalId { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
}
