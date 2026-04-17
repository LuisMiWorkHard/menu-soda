namespace MenuSoda.Application.Dto;

public class EntradaCreateRequest
{
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int TipoEntradaId { get; set; }
    public int? ImagenId { get; set; }
}
