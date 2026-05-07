namespace MenuSoda.Application.Dto;

public class MenuDiarioImagenInsertRequest
{
    public int MenuDiarioId { get; set; }
    public int ImagenId { get; set; }
    public int? MenuImagenId { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
}
