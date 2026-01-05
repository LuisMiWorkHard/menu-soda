namespace MenuSoda.Application.Dto
{
    public class UsuarioGetByDocumentRequest
    {
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = "";
    }
}