namespace MenuSoda.Domain.Models.Security
{
    public class RefreshTokenCreateRequest
    {
        public int UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceId { get; set; }
        public decimal? GeoLat { get; set; }
        public decimal? GeoLon { get; set; }
        public int DaysToExpire { get; set; } = 15;
    }
}