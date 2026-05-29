using Dapper;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using MenuSoda.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Data;

namespace MenuSoda.Application.UseCases.Auth;

public class EnviarCodigoRecuperacionUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly GenericRepository _genericRepository;
    private readonly IEmailService _emailService;
    private readonly ResendOptions _resendOptions;

    public EnviarCodigoRecuperacionUseCase(
        IUserRepository userRepository,
        GenericRepository genericRepository,
        IEmailService emailService,
        IOptions<ResendOptions> resendOptions)
    {
        _userRepository    = userRepository;
        _genericRepository = genericRepository;
        _emailService      = emailService;
        _resendOptions     = resendOptions.Value;
    }

    public async Task<EnviarCodigoRecuperacionResponse> ExecuteAsync(int userId, CancellationToken ct)
    {
        // 1. Obtener usuario por ID
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        // 2. Verificar límite de reenvíos
        var countResult = await _genericRepository.GetSingleByProcedureAsync<CountResult>(
            "seguridad.sp_count_tokens_sin_usar",
            new { p_usuario_id = userId },
            ct
        );

        if ((countResult?.Count ?? 0) >= _resendOptions.MaxReenvios)
        {
            var tiempoResult = await _genericRepository.GetSingleByProcedureAsync<TiempoDisponibleResult>(
                "seguridad.sp_get_tiempo_disponible_recuperacion",
                new { p_usuario_id = userId },
                ct
            );
            var minutosRestantes = tiempoResult?.FecDisponible.HasValue == true
                ? (int)Math.Ceiling((tiempoResult.FecDisponible.Value - DateTime.UtcNow).TotalMinutes)
                : 5;
            throw new TooManyRequestsException(
                $"Has superado el límite de {_resendOptions.MaxReenvios} envíos. " +
                $"Intenta nuevamente en {minutosRestantes} minuto(s).");
        }

        // 3. Anular tokens anteriores activos
        var anularParams = new DynamicParameters();
        anularParams.Add("p_usuario_id", userId, DbType.Int32);
        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_anular_tokens_recuperacion", anularParams, ct);

        // 4. Generar código de 4 dígitos y guardar (expira en 5 min)
        var codigo  = Random.Shared.Next(1000, 10000).ToString("D4");
        var fecVenc = DateTime.UtcNow.AddMinutes(5);

        var insertParams = new DynamicParameters();
        insertParams.Add("p_usuario_id", userId,  DbType.Int32);
        insertParams.Add("p_token",      codigo,   DbType.String);
        insertParams.Add("p_fec_venc",   fecVenc,  DbType.DateTime);
        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_insert_token_recuperacion", insertParams, ct);

        // 5. Enviar email con código
        var nombre = string.IsNullOrWhiteSpace(user.Usunom) ? user.Usuemail : user.Usunom;
        await _emailService.SendVerificationCodeAsync(user.Usuemail, nombre, codigo, ct);

        // 6. Retornar email enmascarado
        return new EnviarCodigoRecuperacionResponse { EmailMasked = MaskEmail(user.Usuemail) };
    }

    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "****";
        var local = parts[0];
        var masked = local.Length <= 2
            ? new string('*', local.Length)
            : $"{local[0]}{new string('*', local.Length - 2)}{local[^1]}";
        return $"{masked}@{parts[1]}";
    }
}
