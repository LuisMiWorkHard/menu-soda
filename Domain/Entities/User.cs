namespace MenuSoda.Domain.Users;

public class User
{
    public int Id { get; set; }
    public int Usutipdoc { get; set; }
    public string Usunumdoc { get; set; } = "";
    public string Usunom { get; set; } = "";
    public string Usuapepat { get; set; } = "";
    public string Usuapemat { get; set; } = "";
    public char Ususexo { get; set; }
    public string Usufecnac { get; set; } = "";
    public string Usuemail { get; set; } = "";
    public string Usutel { get; set; } = "";
    public string? Usudir { get; set; }
    public int? Usudis { get; set; }
    public int? Usupro { get; set; }
    public int? Usudep { get; set; }
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }
    public string Usuhash { get; set; } = "";
}