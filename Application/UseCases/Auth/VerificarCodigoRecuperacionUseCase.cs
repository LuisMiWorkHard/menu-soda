using Dapper;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Exceptions;
using System.Data;

namespace MenuSoda.Application.UseCases.Auth;

public class VerificarCodigoRecuperacionUseCase
{
    private readonly GenericRepository _genericRepository;
    private const int MaxIntentos = 5;

    public VerificarCodigoRecuperacionUseCase(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task ExecuteAsync(int userId, VerificarCodigoRecuperacionRequest request, CancellationToken ct)
    {
        var verParams = new DynamicParameters();
        verParams.Add("p_usuario_id",   userId,           DbType.Int32);
        verParams.Add("p_codigo",       request.Codigo,   DbType.String);
        verParams.Add("p_max_intentos", MaxIntentos,      DbType.Int32);

        var result = await _genericRepository.GetSingleByProcedureAsync<VerificacionResult>(
            "seguridad.sp_verificar_codigo_recuperacion", verParams, ct);

        if (result == null || !result.Valido)
        {
            var msg = result?.Motivo switch
            {
                "sin_token"        => "No hay un código activo o ha expirado.",
                "agotado_intentos" => "Has superado el número máximo de intentos. Solicita un nuevo código.",
                _                  => result?.IntentosRestantes > 0
                    ? $"Código incorrecto. Intentos restantes: {result.IntentosRestantes}."
                    : "Código incorrecto."
            };
            throw new CustomBusinessValidationException(msg);
        }
    }
}
