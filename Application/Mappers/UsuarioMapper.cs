using System.Globalization;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Mappers;

public static class UsuarioMapper
{
    public static UsuarioResponse ToUsuarioResponse(this User entity)
    {
        var nombreCompleto = string.Join(" ", new[] { entity.Usunom, entity.Usuapepat, entity.Usuapemat }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        var tipoDoc = entity.Usutipdoc switch
        {
            1 => "DNI",
            2 => "CE",
            _ => $"Tipo {entity.Usutipdoc}"
        };

        var genero = entity.Ususexo switch
        {
            'M' => "Masculino",
            'F' => "Femenino",
            _ => ""
        };

        var fechaNacimiento = "";
        if (DateTime.TryParse(entity.Usufecnac, out var fechaNac))
        {
            fechaNacimiento = fechaNac.ToString("d 'de' MMMM 'de' yyyy", new CultureInfo("es-ES"));
        }

        return new UsuarioResponse
        {
            NombreCompleto = CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(nombreCompleto.Trim().ToLowerInvariant()),
            Documento = $"{tipoDoc} {entity.Usunumdoc.Trim()}",
            Email = entity.Usuemail.Trim().ToLowerInvariant(),
            Telefono = entity.Usutel,
            Genero = genero,
            FechaNacimiento = fechaNacimiento,
            DireccionCasa = string.IsNullOrWhiteSpace(entity.Usudir) ? null : entity.Usudir.Trim()
        };
    }
}
