namespace MenuSoda.Domain.Models.Security
{
    public class RefreshTokenRotateResponse
    {
        public string PlainText { get; set; } = null!;
        public DateTime ExpiresUtc { get; set; }
        public Guid TokenId { get; set; }
    }
}