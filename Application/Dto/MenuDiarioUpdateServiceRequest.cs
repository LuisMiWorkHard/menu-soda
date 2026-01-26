namespace MenuSoda.Application.Dto;

public class MenuDiarioUpdateServiceRequest
{
    public int Id { get; set; }
    public DateTime Mendiafec { get; set; }
    public int Codest { get; set; }
    public List<int> EntradasIds { get; set; } = new();
    public List<MenuDiarioPlatoRequest> Platos { get; set; } = new();
    public int ImagenId { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
