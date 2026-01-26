namespace MenuSoda.Application.Dto;

public class ImagenUpdateServiceRequest
{
    public int Id { get; set; }
    public string Imarut { get; set; } = string.Empty;
    public string Imanom { get; set; } = string.Empty;
    public string Imaext { get; set; } = string.Empty;
    public int Codest { get; set; }
    public string Usumod { get; set; } = string.Empty;
}
