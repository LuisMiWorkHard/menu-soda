namespace MenuSoda.Application.Dto;

public class MenuDiarioEntradaInsertRequest
{
    public int MenuDiarioId { get; set; }
    public int EntradaId { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
}
