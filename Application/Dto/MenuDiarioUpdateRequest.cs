namespace MenuSoda.Application.Dto;

public class MenuDiarioUpdateRequest
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int EstadoId { get; set; }
    public List<int> EntradasIds { get; set; } = new();
    public List<MenuDiarioPlatoRequest> Platos { get; set; } = new();
    public int? ImagenId { get; set; }
}
