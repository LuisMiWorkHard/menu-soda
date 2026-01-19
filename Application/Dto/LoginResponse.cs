namespace MenuSoda.Application.Dto
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiresUtc { get; set; }
    }
}