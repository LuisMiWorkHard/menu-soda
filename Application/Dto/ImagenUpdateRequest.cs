namespace MenuSoda.Application.Dto;

public class ImagenUpdateRequest
{
    public int Id { get; set; }
    public string Imarut { get; set; } = string.Empty;
    public string Imanom { get; set; } = string.Empty;
    public string Imaext { get; set; } = string.Empty;
    public int Codest { get; set; }
}
