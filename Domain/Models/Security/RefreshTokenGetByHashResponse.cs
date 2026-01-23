namespace MenuSoda.Domain.Models.Security
{
    public class RefreshTokenGetByHashResponse
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public string? DeviceId { get; set; }
    }
}