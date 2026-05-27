using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Mappers;

public static class PerfilMapper
{
    public static PerfilResponse ToResponse(this User entity) =>
        new()
        {
            NombreCompleto = string.Join(" ", new[] { entity.Usunom, entity.Usuapepat, entity.Usuapemat }
                .Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
            Email = entity.Usuemail,
            Telefono = entity.Usutel
        };
}
