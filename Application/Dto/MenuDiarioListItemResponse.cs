namespace MenuSoda.Application.Dto;

public class MenuDiarioListItemResponse
{
    public int Id { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string DescripcionFecha { get; set; } = string.Empty;
    public int EstadoId { get; set; }
    public int CantidadEntradas { get; set; }
    public List<TipoPlatoCount> CantidadPlatos { get; set; } = new();
    public string TiempoTranscurrido { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public List<CoincidenciaDto>? Coincidencias { get; set; }
}

public class TipoPlatoCount
{
    public string TipoPlato { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class CoincidenciaDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
