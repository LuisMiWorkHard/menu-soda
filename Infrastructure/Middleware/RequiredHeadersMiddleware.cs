using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MenuSoda.Infrastructure.Middleware;

public class RequiredHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] Required = ["User-Agent", "DeviceId"];

    public RequiredHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        foreach (var h in Required)
        {
            if (!context.Request.Headers.TryGetValue(h, out var v) || string.IsNullOrWhiteSpace(v))
            {
                var factory = context.RequestServices.GetService<ProblemDetailsFactory>();
                ProblemDetails problem = factory?.CreateProblemDetails(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Encabezado requerido faltante",
                    detail: $"Falta el header '{h}'."
                ) ?? new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Encabezado requerido faltante",
                    Detail = $"Falta el header '{h}'."
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
