using System.Globalization;
using System.Text.Json;
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
                DateTime tmp;
                if (DateTime.TryParseExact(fecModStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
                    fecModVal = tmp;
            }

            var platosList = new List<TipoPlatoCount>();
            if (item.Platos_por_tipo != null)
            {
                try
                {
                    platosList = JsonSerializer.Deserialize<List<TipoPlatoCount>>(item.Platos_por_tipo)
                                 ?? new List<TipoPlatoCount>();
                }
                catch { /* Ignorar error de parseo */ }
            }

            string? imagenUrl = null;
            if (!string.IsNullOrEmpty(item.Imagen_ruta))
            {
                try { imagenUrl = _storageService.GetSignedUrl(item.Imagen_ruta); }
                catch { /* No interrumpir el listado si falla la URL firmada */ }
            }

            response.Add(new MenuDiarioListItemResponse
            {
                Id = item.Id,
                Fecha = fechaStr,
                EstadoId = item.Codest,
                DescripcionFecha = GetFriendlyDateName(fechaVal),
                TiempoTranscurrido = GetTimeElapsed(fecModVal),
                CantidadEntradas = item.Cantidad_entradas ?? 0,
                CantidadPlatos = platosList,
                ImagenUrl = imagenUrl
            });
        }

        return response;
    }

    private string GetFriendlyDateName(DateTime date)
    {
        var today = DateTime.Today;
        if (date.Date == today) return "Hoy";
        if (date.Date == today.AddDays(-1)) return "Ayer";
        if (date.Date == today.AddDays(1)) return "Mañana";

        var cal = CultureInfo.CurrentCulture.Calendar;
        var startOfWeekToday = today.AddDays(-1 * (int)cal.GetDayOfWeek(today));
        var startOfWeekDate  = date.AddDays(-1 * (int)cal.GetDayOfWeek(date));

        var es = CultureInfo.GetCultureInfo("es-ES");
        if (startOfWeekToday == startOfWeekDate)
            return es.TextInfo.ToTitleCase(date.ToString("dddd", es));

        // Formato: "Lunes, 01 May 2026"
        string dayName   = es.TextInfo.ToTitleCase(date.ToString("dddd", es));
        string monthAbbr = es.TextInfo.ToTitleCase(date.ToString("MMM", es).TrimEnd('.'));
        return $"{dayName}, {date.Day:D2} {monthAbbr} {date.Year}";
    }

    private string GetTimeElapsed(DateTime? lastMod)
    {
        if (!lastMod.HasValue) return "";
        var timeSpan = DateTime.Now - lastMod.Value;

        if (timeSpan.TotalMinutes < 60) return $"Hace {timeSpan.TotalMinutes:0} min";
        if (timeSpan.TotalHours < 24) return $"Hace {timeSpan.TotalHours:0} horas";
        if (timeSpan.TotalDays < 7) return $"Hace {timeSpan.TotalDays:0} días";
        if (timeSpan.TotalDays < 30) return $"Hace {(int)(timeSpan.TotalDays / 7)} semanas";

        return lastMod.Value.ToString("dd/MM/yyyy");
    }
}
