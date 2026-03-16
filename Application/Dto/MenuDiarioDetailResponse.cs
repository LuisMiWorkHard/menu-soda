using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Dto;

/// <summary>
/// Respuesta detallada del menú diario con todas sus relaciones
/// </summary>
public class MenuDiarioDetailResponse
{
    public int Id { get; set; }
    public string Mendiafec { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Fecreg { get; set; } = string.Empty;
    public string Usureg { get; set; } = string.Empty;
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }

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
