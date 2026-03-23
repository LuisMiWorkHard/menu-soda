using System.Globalization;
using System.Text.Json;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class ListarMenusDiariosUseCase
{
    private readonly IMenuDiarioRepository _menuDiarioRepository;

    public ListarMenusDiariosUseCase(IMenuDiarioRepository menuDiarioRepository)
    {
        _menuDiarioRepository = menuDiarioRepository;
    }

    public async Task<IEnumerable<MenuDiarioListItemResponse>> ExecuteAsync(CancellationToken ct)
    {
        var reportData = await _menuDiarioRepository.GetCustomListReportAsync(ct);

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
                {
                    fecModVal = tmp;
                }
            }

            var platosList = new List<TipoPlatoCount>();
            if (item.Platos_por_tipo != null)
            {
                string json = item.Platos_por_tipo;
                try
                {
                    platosList = JsonSerializer.Deserialize<List<TipoPlatoCount>>(json)
                                ?? new List<TipoPlatoCount>();
                }
                catch { /* Ignorar error de parseo */ }
            }

            response.Add(new MenuDiarioListItemResponse
            {
                Id = item.Id,
                Fecha = fechaStr,
                EstadoId = item.Codest,
                DescripcionFecha = GetFriendlyDateName(fechaVal),
                TiempoTranscurrido = GetTimeElapsed(fecModVal),
                CantidadEntradas = item.Cantidad_entradas ?? 0,
                CantidadPlatos = platosList
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
        var d1 = today.Date.AddDays(-1 * (int)cal.GetDayOfWeek(today));
        var d2 = date.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date));
        if (d1 == d2) return date.ToString("dddd");

        return date.ToString("dd MMMM yyyy").ToUpper();
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
