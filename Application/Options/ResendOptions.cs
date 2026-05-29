namespace MenuSoda.Application.Options;

public class ResendOptions
{
    public string ApiKey           { get; set; } = "";
    public string FromEmail        { get; set; } = "";
    public string FromName         { get; set; } = "MenuSoda";
    public string ResetPasswordUrl { get; set; } = "";
    public int    MaxReenvios      { get; set; } = 3;
}
