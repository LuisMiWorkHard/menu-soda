public class RefreshTokenCreateRequest
{
    public int UserId { get; set; }
    public string? IpAddress { get; set; }
    public int DaysToExpire { get; set; } = 15;
}