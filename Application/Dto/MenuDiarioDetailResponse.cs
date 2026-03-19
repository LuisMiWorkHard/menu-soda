using MenuSoda.Domain.Models.Repositories;

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
    public int Codpla { get; set; }
    public string Planom { get; set; } = string.Empty;
    public string? Plades { get; set; }
    public int Codtippla { get; set; }
    public string Tipplades { get; set; } = string.Empty;
    public int Codest { get; set; }
    public MenuDiarioPlatoAdicionalResponse? Adicional { get; set; }
}
