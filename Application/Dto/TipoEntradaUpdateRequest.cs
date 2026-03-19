namespace MenuSoda.Application.Dto;

public class TipoEntradaUpdateRequest
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
