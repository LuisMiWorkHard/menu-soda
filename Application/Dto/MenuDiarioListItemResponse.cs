namespace MenuSoda.Application.Dto;

public class MenuDiarioListItemResponse
{
    public int Id { get; set; }
    public string Mendiafec { get; set; } = string.Empty;
    public string NombreFecha { get; set; } = string.Empty; // Hoy, Ayer, Lunes 20...
    public int Codest { get; set; }
    
    public int CantidadEntradas { get; set; }
    public List<TipoPlatoCount> CantidadPlatosPorTipo { get; set; } = new();
    
    public string TiempoTranscurrido { get; set; } = string.Empty; // Hace x min...
}

public class TipoPlatoCount
{
    public string TipoPlato { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
