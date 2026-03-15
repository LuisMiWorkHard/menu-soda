namespace MenuSoda.Application.Options;

public class GcsOptions
{
    public string BucketName { get; set; } = "";
    public int SignedUrlExpirationMinutes { get; set; } = 60;
    public int MaxImageSizeMb { get; set; } = 5;
}
