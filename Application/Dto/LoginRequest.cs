namespace MenuSoda.Application.Dto
{
    public class LoginRequest
    {
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = "";
        public string Contrasena { get; set; } = "";
    }
}