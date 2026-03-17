namespace MenuSoda.Application.Dto;

public class EntradaUpdateRequest
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = "";
    public string DescripcionLarga { get; set; } = "";
    public int TipoEntradaId { get; set; }
    public int EstadoId { get; set; }
    public int ImagenId { get; set; }
}
