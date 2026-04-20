using System.Text.Json.Serialization;

namespace MenuSoda.Application.Dto;

public class MenuImagenResponse
{
    public int Id { get; set; }
    public int ImagenId { get; set; }
    public string ImagenUrl { get; set; } = "";
    [JsonIgnore]
    public string ImagenRuta { get; set; } = "";
    [JsonIgnore]
    public string ImagenExtension { get; set; } = "";
    public int EstadoId { get; set; }
    public string FechaRegistro { get; set; } = "";
    public string UsuarioRegistro { get; set; } = "";
    public string? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public decimal Aretextop { get; set; }
    public decimal Aretexbot { get; set; }
    public decimal Aretexini { get; set; }
    public decimal Aretexfin { get; set; }
    public decimal Maxfonsiz { get; set; }
    public string  Fonfam    { get; set; } = "";
}
