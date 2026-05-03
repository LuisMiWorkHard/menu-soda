using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class ListarMenusDiariosUseCase
{
    private readonly IMenuDiarioRepository _menuDiarioRepository;
    private readonly IStorageService _storageService;

    public ListarMenusDiariosUseCase(IMenuDiarioRepository menuDiarioRepository, IStorageService storageService)
    {
        _menuDiarioRepository = menuDiarioRepository;
        _storageService = storageService;
    }

    public async Task<IEnumerable<MenuDiarioListItemResponse>> ExecuteAsync(string? busqueda, CancellationToken ct)
    {
        var reportData = await _menuDiarioRepository.GetCustomListReportAsync(busqueda, ct);

        var response = new List<MenuDiarioListItemResponse>();

        foreach (var item in reportData)
        {
            string fechaStr = item.Mendiafec;
            string? fecModStr = item.Fecmod ?? item.Fecreg;

            DateTime fechaVal;
            DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVal);

            DateTime? fecModVal = null;
            if (!string.IsNullOrEmpty(fecModStr))
            {
                if (DateTime.TryParse(fecModStr, null, DateTimeStyles.RoundtripKind, out var tmp))
                    fecModVal = tmp.ToUniversalTime();
            }

            var platosList = new List<TipoPlatoCount>();
            if (item.Platos_por_tipo != null)
            {
                try
                {
                    var raw = JsonSerializer.Deserialize<List<PlatosPorTipoJson>>(item.Platos_por_tipo);
                    if (raw != null)
                        platosList = raw.Select(p => new TipoPlatoCount { TipoPlato = p.TipoPlato, Cantidad = p.Cantidad }).ToList();
                }
                catch { }
            }

            string? imagenUrl = null;
            if (!string.IsNullOrEmpty(item.Imagen_ruta))
            {
                try { imagenUrl = _storageService.GetSignedUrl(item.Imagen_ruta); }
                catch { }
            }

            List<CoincidenciaDto>? coincidencias = null;
            if (!string.IsNullOrEmpty(item.Coincidencias))
            {
                try
                {
                    var raw = JsonSerializer.Deserialize<List<CoincidenciaJson>>(item.Coincidencias);
                    if (raw != null)
                        coincidencias = raw.Select(c => new CoincidenciaDto { Tipo = c.Tipo, Nombre = c.Nombre }).ToList();
                }
                catch { }
            }

            response.Add(new MenuDiarioListItemResponse
            {
                Id = item.Id,
                Fecha = fechaStr,
                EstadoId = item.Codest,
                DescripcionFecha = "",
                TiempoTranscurrido = GetTimeElapsed(fecModVal),
                CantidadEntradas = item.Cantidad_entradas ?? 0,
                CantidadPlatos = platosList,
                ImagenUrl = imagenUrl,
                Coincidencias = coincidencias
            });
        }

        return response;
    }

    private string GetTimeElapsed(DateTime? lastMod)
    {
        if (!lastMod.HasValue) return "";
        var span = DateTime.UtcNow - lastMod.Value;
        if (span.TotalSeconds < 60) return "Hace un momento";
        if (span.TotalMinutes < 60) return $"Hace {(int)span.TotalMinutes} min";
        if (span.TotalHours < 24)   return $"Hace {(int)span.TotalHours} horas";
        if (span.TotalDays < 7)     return $"Hace {(int)span.TotalDays} días";
        if (span.TotalDays < 30)    return $"Hace {(int)(span.TotalDays / 7)} semanas";
        return lastMod.Value.ToString("dd/MM/yyyy");
    }

    private class PlatosPorTipoJson
    {
        [JsonPropertyName("tipo_plato")] public string TipoPlato { get; set; } = string.Empty;
        [JsonPropertyName("cantidad")]   public int Cantidad { get; set; }
    }

    private class CoincidenciaJson
    {
        [JsonPropertyName("tipo")]   public string Tipo   { get; set; } = string.Empty;
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = string.Empty;
    }
}
