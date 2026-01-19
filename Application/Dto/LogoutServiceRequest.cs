namespace MenuSoda.Application.Dto
{
    public class LogoutServiceRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string? IpAddress { get; set; }
    }
}