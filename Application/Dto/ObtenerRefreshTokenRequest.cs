namespace MenuSoda.Application.Dto
{
    public class ObtenerRefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceId { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }
}