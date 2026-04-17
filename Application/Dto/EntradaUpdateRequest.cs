namespace MenuSoda.Application.Dto;

public class EntradaUpdateRequest
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int TipoEntradaId { get; set; }
    public int EstadoId { get; set; }
    public int ImagenId { get; set; }
}
