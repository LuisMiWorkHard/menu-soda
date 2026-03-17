namespace MenuSoda.Application.Dto;

public class EntradaCreateRequest
{
    public string Descripcion { get; set; } = "";
    public string DescripcionLarga { get; set; } = "";
    public int TipoEntradaId { get; set; }
    public int ImagenId { get; set; }
}
