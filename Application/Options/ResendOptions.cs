namespace MenuSoda.Application.Options;

public class ResendOptions
{
    public string FromEmail        { get; set; } = "onboarding@resend.dev";
    public string FromName         { get; set; } = "MenuSoda";
    public string ResetPasswordUrl { get; set; } = "";
    public int    MaxReenvios      { get; set; } = 3;
}
