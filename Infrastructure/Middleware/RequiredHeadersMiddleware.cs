using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MenuSoda.Application.Options;

namespace MenuSoda.Infrastructure.Middleware;

public class RequiredHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _baseRequired;
    private readonly string _geoLatHeader;
    private readonly string _geoLonHeader;

    public RequiredHeadersMiddleware(RequestDelegate next, IOptions<AuthOptions> options)
    {
        _next = next;
        var authOptions = options.Value;
        
        _baseRequired = new string[]
        {
            "User-Agent",
            authOptions.DeviceIdHeaderName
        }
        .Where(h => !string.IsNullOrWhiteSpace(h))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

        _geoLatHeader = authOptions.GeoLatHeaderName;
        _geoLonHeader = authOptions.GeoLonHeaderName;
    }

    public async Task Invoke(HttpContext context)
    {
        var requiredForThisRequest = new List<string>(_baseRequired);

        // Verificar si la ruta es la de login para exigir GeoLat y GeoLon
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.EndsWith("/login", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(_geoLatHeader)) requiredForThisRequest.Add(_geoLatHeader);
            if (!string.IsNullOrWhiteSpace(_geoLonHeader)) requiredForThisRequest.Add(_geoLonHeader);
        }

        foreach (var h in requiredForThisRequest)
        {
            if (!context.Request.Headers.TryGetValue(h, out var v) || string.IsNullOrWhiteSpace(v))
            {
                var factory = context.RequestServices.GetService<ProblemDetailsFactory>();
                ProblemDetails problem = factory?.CreateProblemDetails(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Encabezado requerido faltante",
                    detail: $"Petición invalida. No se encontro el encabezado '{h}'."
                ) ?? new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Encabezado requerido faltante",
                    Detail = $"Petición invalida. No se encontro el encabezado '{h}'."
                };

                problem.Extensions["code"] = "ERR_HEADER_REQUERIDO";
                problem.Extensions["missingHeader"] = h;

                context.Response.StatusCode = problem.Status ?? StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }
        }

        await _next(context);
    }
}
