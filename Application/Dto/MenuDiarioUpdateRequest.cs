namespace MenuSoda.Application.Dto;

public class MenuDiarioUpdateRequest
{
    public int Id { get; set; }
    public DateTime Mendiafec { get; set; }
    public int Codest { get; set; }
    public List<int> EntradasIds { get; set; } = new();
    public List<MenuDiarioPlatoRequest> Platos { get; set; } = new();
    public int ImagenId { get; set; }
}
