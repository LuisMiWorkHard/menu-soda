namespace MenuSoda.Application.Dto;

public class PlatoCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string DescripcionLarga { get; set; } = "";
    public int TipoPlatoId { get; set; }
    public int? ImagenId { get; set; }
}
