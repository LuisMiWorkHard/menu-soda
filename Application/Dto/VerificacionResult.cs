namespace MenuSoda.Application.Dto;

public class VerificacionResult
{
    public bool   Valido            { get; set; }
    public string Motivo            { get; set; } = "";
    public int    IntentosRestantes { get; set; }
}
