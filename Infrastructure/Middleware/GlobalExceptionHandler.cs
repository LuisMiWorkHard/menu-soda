using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MenuSoda.Infrastructure.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;
    private readonly int _maxImageSizeMb;

    public GlobalExceptionHandler(
        ProblemDetailsFactory problemDetailsFactory,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment env,
        IConfiguration configuration)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _logger = logger;
        _env = env;
        _maxImageSizeMb = configuration.GetValue("Gcs:MaxImageSizeMb", 5);
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            BadHttpRequestException badReq     => badReq.StatusCode,
            ArgumentException                  => StatusCodes.Status400BadRequest,
            FormatException                    => StatusCodes.Status400BadRequest,
            ValidationException                => StatusCodes.Status400BadRequest,
            System.Text.Json.JsonException     => StatusCodes.Status400BadRequest,

            System.Security.SecurityException  => StatusCodes.Status403Forbidden,

            KeyNotFoundException               => StatusCodes.Status404NotFound,
            FileNotFoundException              => StatusCodes.Status404NotFound,
            DirectoryNotFoundException         => StatusCodes.Status404NotFound,

            TimeoutException                   => StatusCodes.Status408RequestTimeout,
            TaskCanceledException              => StatusCodes.Status408RequestTimeout,
            OperationCanceledException         => StatusCodes.Status408RequestTimeout,

            UnauthorizedAccessException        => StatusCodes.Status401Unauthorized,
            Npgsql.PostgresException pgEx when pgEx.SqlState == "P0001" => StatusCodes.Status400BadRequest,
            CustomBusinessValidationException  => StatusCodes.Status409Conflict,

            _                                  => StatusCodes.Status500InternalServerError
        };

        _logger.LogError(exception, "Excepción no controlada");

        var title = MapTitle(exception);

        var problem = _problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: status,
            title: title,
            instance: httpContext.Request.Path
        );

        if (exception is Npgsql.PostgresException pgValEx && pgValEx.SqlState == "P0001")
        {
            problem.Detail = pgValEx.MessageText;
        }
        else if (_env.IsDevelopment())
        {
            problem.Detail = status == StatusCodes.Status500InternalServerError
                ? $"{exception.GetType().Name}: {exception.Message} - {exception.InnerException?.Message}"
                : exception.Message;
        }
        else
        {
            problem.Detail = MapProductionDetail(exception, status);
        }

        problem.Extensions["code"] = MapCode(exception);
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static string MapCode(Exception ex) => ex switch
    {
        InvalidDataException                => "ERR_PAYLOAD_TOO_LARGE",
        ArgumentException                   => "ERR_BAD_REQUEST",
        FormatException                     => "ERR_BAD_REQUEST",
        ValidationException                 => "ERR_BAD_REQUEST",
        System.Text.Json.JsonException      => "ERR_BAD_REQUEST",

        System.Security.SecurityException   => "ERR_FORBIDDEN",

        KeyNotFoundException                => "ERR_NOT_FOUND",
        FileNotFoundException               => "ERR_NOT_FOUND",
        DirectoryNotFoundException          => "ERR_NOT_FOUND",

        TimeoutException                    => "ERR_TIMEOUT",
        TaskCanceledException               => "ERR_TIMEOUT",
        OperationCanceledException          => "ERR_TIMEOUT",

        UnauthorizedAccessException         => "ERR_UNAUTHORIZED",
        Npgsql.PostgresException pgEx when pgEx.SqlState == "P0001" => "ERR_BAD_REQUEST",
        CustomBusinessValidationException   => "ERR_BUSINESS",

        _                                   => "ERR_INTERNAL"
    };

    private static string MapTitle(Exception ex) => ex switch
    {
        /*el cuerpo de la solicitud excede el límite permitido*/
        InvalidDataException                => "Archivo demasiado grande",
        /*argumento inválido para un método. Se usa al validar parámetros de entrada (valor fuera de rango, nulo cuando no toca, combinación inválida). Ej.: comprobar x > 0*/
        ArgumentException                   => "Solicitud inválida (argumento incorrecto)",
        /*formato inválido para un dato. Ej.: fecha mal formada, número con letras, cadena no convertible a enum*/
        FormatException                     => "Formato de datos inválido",
        /*error de validación de datos. Usado por DataAnnotations y FluentValidation*/
        ValidationException                 => "Error de validación de datos",
        System.Text.Json.JsonException      => "JSON mal formado",

        System.Security.SecurityException   => "Sin permiso al recurso",
        
        KeyNotFoundException                => "Recurso no encontrado",
        FileNotFoundException               => "Archivo no encontrado",
        DirectoryNotFoundException          => "Directorio no encontrado",
        
        TimeoutException                    => "Tiempo de espera excedido",
        TaskCanceledException               => "Operación cancelada por timeout",
        OperationCanceledException          => "Operación cancelada",

        UnauthorizedAccessException         => "No autorizado",
        Npgsql.PostgresException pgEx when pgEx.SqlState == "P0001" => "Solicitud inválida (error de base de datos)",
        CustomBusinessValidationException   => "Reglas de negocio insatisfechas",

        _                                   => "Error interno del servidor"
    };

    private string MapProductionDetail(Exception ex, int status) => status switch
    {
        StatusCodes.Status413PayloadTooLarge       => $"El archivo excede el tamaño máximo permitido ({_maxImageSizeMb} MB).",
        StatusCodes.Status400BadRequest            => "La solicitud contiene datos inválidos.",
        StatusCodes.Status401Unauthorized          => "Debe autenticarse para continuar.",
        StatusCodes.Status403Forbidden             => "No tiene permisos para esta operación.",
        StatusCodes.Status404NotFound              => "El recurso solicitado no existe.",
        StatusCodes.Status408RequestTimeout        => "La solicitud excedió el tiempo de espera.",
        StatusCodes.Status409Conflict              => ex is CustomBusinessValidationException
                                                        ? ex.Message // mensaje de negocio (si es seguro)
                                                        : "La solicitud conflicta con el estado actual del recurso.",
        _                                          => "Se produjo un error inesperado en el servidor."
    };

    public sealed class CustomBusinessValidationException : Exception
    {
        public CustomBusinessValidationException(string message) : base(message) {}
        public CustomBusinessValidationException(string message, Exception inner) : base(message, inner) {}
        
    }
}