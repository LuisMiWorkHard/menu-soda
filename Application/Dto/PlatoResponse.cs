using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Dto;

public class PlatoResponse
{
    public int Id { get; set; }
    public string Planom { get; set; } = "";
    public string Plades { get; set; } = "";
    public int Codtippla { get; set; }
    public int Codest { get; set; }
    public string Fecreg { get; set; } = "";
    public string Usureg { get; set; } = "";
    public string? Fecmod { get; set; }
    public string? Usumod { get; set; }

    public static PlatoResponse FromEntity(Plato entity)
    {
        return new PlatoResponse
        {
            Id = entity.Id,
            Planom = entity.Planom,
            Plades = entity.Plades,
            Codtippla = entity.Codtippla,
            Codest = entity.Codest,
            Fecreg = entity.Fecreg,
            Usureg = entity.Usureg,
            Fecmod = entity.Fecmod,
            Usumod = entity.Usumod
        };
    }
}
