namespace MenuSoda.Application.Dto
{
    public class LoginRequest
    {
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceId { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }
}
