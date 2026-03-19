namespace MenuSoda.Application.Dto;

public class MenuDiarioCreateRequest
{
    public DateTime Fecha { get; set; }
    public List<int> EntradasIds { get; set; } = new();
    public List<MenuDiarioPlatoRequest> Platos { get; set; } = new();
    public int ImagenId { get; set; }
}
