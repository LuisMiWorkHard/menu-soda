namespace MenuSoda.Application.Dto
{
    public class RefreshTokenRotateRequest
    {
        public Guid OldTokenId { get; set; }
        public int UserId { get; set; }
        public string? IpAddress { get; set; }
        public int DaysToExpire { get; set; } = 15;
    }
}