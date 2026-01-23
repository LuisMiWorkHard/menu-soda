using MenuSoda.Application.Services;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware;
using MenuSoda.Infrastructure.Persistence;
using MenuSoda.Infrastructure.Security;
using MenuSoda.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MenuSoda.Application.Options;
using Microsoft.AspNetCore.HttpOverrides;
using FluentValidation;
using FluentValidation.AspNetCore;
using MenuSoda.Application.Validators;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DapperContext>();
// Add services to the container.
builder.Services.AddScoped<GenericRepository>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenGenerator>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var secret = config.GetSection("Jwt:Secret").Value!;
    if (string.IsNullOrWhiteSpace(secret))
        throw new InvalidOperationException("Falta configuración Jwt:Secret.");

    var tokenDuration = config.GetSection("Jwt:AccessTokenMinutes").Value!;
    if (string.IsNullOrWhiteSpace(tokenDuration))
        throw new InvalidOperationException("Falta configuración Jwt:AccessTokenMinutes.");

    return new JwtTokenGenerator(secret, tokenDuration);
});

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["Jwt:Secret"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "MenuSodaAPI",
            ValidateAudience = true,
            ValidAudience = "MenuSodaClients",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// Registrar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UtilService>();

builder.Services.AddScoped<IRefreshToken, DapperRefreshTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
builder.Services.AddScoped<IEntradaService, EntradaService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Rate Limiting para prevenir ataques de fuerza bruta
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    // Política específica para endpoints de autenticación
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,              // 5 requests
                Window = TimeSpan.FromMinutes(1), // por minuto
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                // Sin cola, rechazar inmediatamente
            }));
});

// Habilita ProblemDetails (RFC 7807) y añade traceId a todos
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
    };
});

// Configura opciones fuertemente tipadas desde appsettings.json
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));

// Configura respuesta personalizada para validación de modelos
builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.InvalidModelStateResponseFactory = ctx =>
    {
        var factory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var problem = factory.CreateValidationProblemDetails(
            ctx.HttpContext,
            ctx.ModelState,
            statusCode: StatusCodes.Status422UnprocessableEntity,
            title: "Datos inválidos",
            detail: "La solicitud contiene datos inválidos"
        );

        // Campo propio para clientes
        problem.Extensions["code"] = "ERR_VALIDACION";

        return new ObjectResult(problem) { StatusCode = problem.Status };
    };
});

// Para obtener IP real cuando hay un proxy adelante
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // En hosting tipo Railway suele variar la IP del proxy; para que funcione:
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Manejador global de excepciones
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Activa el pipeline de manejo de excepciones
app.UseExceptionHandler();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

// Valida headers requeridos para todas las solicitudes
app.UseMiddleware<RequiredHeadersMiddleware>();

// Rate limiting para prevenir ataques de fuerza bruta
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
