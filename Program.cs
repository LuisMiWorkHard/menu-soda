using MenuSoda.Application.Services;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Infrastructure.Middleware;
using MenuSoda.Infrastructure.Persistence;
using MenuSoda.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DapperContext>();
// Add services to the container.
builder.Services.AddScoped<GenericRepository>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenGenerator>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var secret = config.GetSection("Jwt:Secret").Value!;
    return new JwtTokenGenerator(secret);
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Habilita ProblemDetails (RFC 7807) y añade traceId a todos
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
    };
});

builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.InvalidModelStateResponseFactory = ctx =>
    {
        var factory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var problem = factory.CreateValidationProblemDetails(
            ctx.HttpContext,
            ctx.ModelState,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Datos inválidos",
            detail: "La solicitud contiene datos inválidos"
        );

        // Campo propio para clientes
        problem.Extensions["code"] = "ERR_VALIDACION";

        return new ObjectResult(problem) { StatusCode = problem.Status };
    };
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

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

// Valida headers requeridos para todas las solicitudes
app.UseMiddleware<RequiredHeadersMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
