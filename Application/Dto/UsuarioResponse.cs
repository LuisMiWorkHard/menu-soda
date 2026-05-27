namespace MenuSoda.Application.Dto;

public class UsuarioResponse
{
    public string NombreCompleto { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Genero { get; set; } = "";
    public string FechaNacimiento { get; set; } = "";
    public string? DireccionCasa { get; set; }
}
