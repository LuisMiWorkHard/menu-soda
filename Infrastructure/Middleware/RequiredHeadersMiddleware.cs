using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MenuSoda.Application.Options;

namespace MenuSoda.Infrastructure.Middleware;

public class RequiredHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _required;

    public RequiredHeadersMiddleware(RequestDelegate next, IOptions<AuthOptions> options)
    {
        _next = next;
        var authOptions = options.Value;
        _required = new string[]
        {
            "User-Agent",
            authOptions.DeviceIdHeaderName,
            authOptions.GeoLatHeaderName,
            authOptions.GeoLonHeaderName
        }
        .Where(h => !string.IsNullOrWhiteSpace(h))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
    }

    public async Task Invoke(HttpContext context)
    {
        foreach (var h in _required)
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
