namespace MenuSoda.Application.Dto;

public class PlatoUpdateRequest
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TipoPlatoId { get; set; }
    public int EstadoId { get; set; }
}
