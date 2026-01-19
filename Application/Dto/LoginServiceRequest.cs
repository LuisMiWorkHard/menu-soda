namespace MenuSoda.Application.Dto
{
    public class LoginServiceRequest: LoginRequest
    {
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceId { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }
}