using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta detallada del menú diario con todas sus relaciones
/// </summary>
public class MenuDiarioDetailResponse
{
    public int Id { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public int EstadoId { get; set; }
    public string FechaRegistro { get; set; } = string.Empty;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public string? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }

    public List<MenuDiarioEntradaResponse> Entradas { get; set; } = new();
    public List<MenuDiarioPlatoWithAdicionalResponse> Platos { get; set; } = new();
    public MenuDiarioImagenResponse? Imagen { get; set; }
}

/// <summary>
/// DTO que combina la información de un plato con su adicional opcional
/// </summary>
public class MenuDiarioPlatoWithAdicionalResponse
{
    public int Id { get; set; }
    public int PlatoId { get; set; }
    public string PlatoNombre { get; set; } = string.Empty;
    public string? PlatoDescripcion { get; set; }
    public int TipoPlatoId { get; set; }
    public string TipoPlatoDescripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
    public MenuDiarioPlatoAdicionalResponse? Adicional { get; set; }
}
