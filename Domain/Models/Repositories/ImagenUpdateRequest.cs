namespace MenuSoda.Domain.Models.Repositories;

public class ImagenUpdateRequest
{
    public int Id { get; set; }
    public string Imarut { get; set; } = "";
    public string Imanom { get; set; } = "";
    public string Imaext { get; set; } = "";
    public int Codest { get; set; }
    public string Usumod { get; set; } = "";
}
