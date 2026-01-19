namespace MenuSoda.Application.Options
{
    public class AuthOptions
    {
        public int RefreshTokenDaysToExpire { get; set; } = 15;
        public string RefreshTokenCookieName { get; set; } = "refresh_token";
        public string DeviceIdHeaderName { get; set; } = "DeviceId";
        public string GeoLatHeaderName { get; set; } = "GeoLat";
        public string GeoLonHeaderName { get; set; } = "GeoLon";
    }
}