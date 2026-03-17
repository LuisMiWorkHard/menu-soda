namespace MenuSoda.Application.Dto
{
    public class ObtenerRefreshTokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshTokenPlainText { get; set; } = null!;
        public DateTime RefreshTokenExpiresUtc { get; set; }
    }
}