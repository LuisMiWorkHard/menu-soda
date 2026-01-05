public class RefreshTokenCreateResponse
{
    public string? PlainText { get; set; } = null!;
    public DateTime ExpiresUtc { get; set; }
    public Guid TokenId { get; set; }
}