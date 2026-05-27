using System.Globalization;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Mappers;

public static class PerfilMapper
{
    public static PerfilResponse ToResponse(this User entity)
    {
        var nombreCompleto = string.Join(" ", new[] { entity.Usunom, entity.Usuapepat, entity.Usuapemat }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        return new PerfilResponse
        {
            NombreCompleto = CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(nombreCompleto.Trim().ToLowerInvariant()),
            Email = entity.Usuemail.Trim().ToLowerInvariant(),
            Telefono = entity.Usutel
        };
    }
}
