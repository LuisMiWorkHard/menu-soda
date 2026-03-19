namespace MenuSoda.Application.Dto;

public class TipoPlatoUpdateRequest
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
