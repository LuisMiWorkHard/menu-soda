namespace MenuSoda.Application.Dto;

public class MenuDiarioCreateServiceRequest
{
    public DateTime Mendiafec { get; set; }
    public List<int> EntradasIds { get; set; } = new();
    public List<MenuDiarioPlatoRequest> Platos { get; set; } = new();
    public int ImagenId { get; set; }
    public string Usureg { get; set; } = string.Empty;
}
