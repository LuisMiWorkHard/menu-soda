using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using MenuSoda.Infrastructure.Persistence;
using Npgsql;

public class GenericRepository
{
    private readonly DapperContext _context;

    public GenericRepository(DapperContext context)
    {
        _context = context;
    }

    public Task<T?> GetSingleByProcedureAsync<T>(
        string procedureName,
        object procedureParameters,
        CancellationToken ct,
        string cursorName = "result_cur",
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                var safeCursor = ResolveCursorName(cursorName);

                if (!parameters.ParameterNames.Any(n => n.Equals("p_cur", StringComparison.OrdinalIgnoreCase)))
                    parameters.Add("p_cur", safeCursor);

                var names = GetParameterNames(parameters, procedureParameters);
                var callSql = BuildCallSqlNamedArgs(procedureName, names);

                await conn.ExecuteAsync(new CommandDefinition(callSql, parameters, tx, cancellationToken: ct));

                var result = await conn.QuerySingleOrDefaultAsync<T>(
                    new CommandDefinition($"FETCH ALL FROM \"{safeCursor}\";", transaction: tx, cancellationToken: ct));

                await conn.ExecuteAsync(new CommandDefinition($"CLOSE \"{safeCursor}\";", transaction: tx, cancellationToken: ct));

                return result;
            },
            ct,
            transaction
        );
    }

    public Task<IEnumerable<T>?> GetListByProcedureAsync<T>(
        string procedureName,
        object procedureParameters,
        CancellationToken ct,
        string cursorName = "result_cur",
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                var safeCursor = ResolveCursorName(cursorName);

                if (!parameters.ParameterNames.Any(n => n.Equals("p_cur", StringComparison.OrdinalIgnoreCase)))
                    parameters.Add("p_cur", safeCursor);

                var names = GetParameterNames(parameters, procedureParameters);
                var callSql = BuildCallSqlNamedArgs(procedureName, names);

                await conn.ExecuteAsync(new CommandDefinition(callSql, parameters, tx, cancellationToken: ct));

                var result = await conn.QueryAsync<T>(
                    new CommandDefinition($"FETCH ALL FROM \"{safeCursor}\";", transaction: tx, cancellationToken: ct));

                await conn.ExecuteAsync(new CommandDefinition($"CLOSE \"{safeCursor}\";", transaction: tx, cancellationToken: ct));

                return result;
            },
            ct,
            transaction
        );
    }

        public Task CallProcedureAsync(
        string procedureName,
        DynamicParameters parameters,
        CancellationToken ct,
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<int>(
            parameters,
            async (conn, tx, p) =>
            {
                var names = p.ParameterNames
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                var callSql = BuildCallSqlNamedArgs(procedureName, names);

                return await conn.ExecuteAsync(new CommandDefinition(
                    callSql, p, transaction: tx, cancellationToken: ct));
            },
            ct,
            transaction
        );
    }

    public Task<int> ExecuteNonQueryProcedureAsync(
        string procedureName,
        object procedureParameters,
        CancellationToken ct,
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<int>(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                var names = GetParameterNames(parameters, procedureParameters)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                var callSql = BuildCallSqlNamedArgs(procedureName, names);

                return await conn.ExecuteAsync(
                    new CommandDefinition(callSql, parameters, transaction: tx, cancellationToken: ct)
                );
            },
            ct,
            transaction
        );
    }

    private async Task<T?> ExecuteProcedureAsync<T>(
        object procedureParameters,
        Func<NpgsqlConnection, NpgsqlTransaction, DynamicParameters, Task<T?>> executor,
        CancellationToken ct,
        NpgsqlTransaction? transaction = null)
    {
        var ownConnection = transaction == null;
        var conn = ownConnection ? (NpgsqlConnection)_context.CreateConnection() : transaction!.Connection!;
        
        // Si la conexión es propia, debemos asegurarnos de liberarla. Si es externa, NO debemos liberarla aquí.
        // Usaremos un bloque try/finally manual para controlar esto o un using condicional (difícil en C# 8).
        // Mejor enfoque: Asignar conn fuera y liberar solo si ownConnection.
        var tx = transaction;

        if (ownConnection)
        {
            await conn.OpenAsync(ct);
            tx = await conn.BeginTransactionAsync(ct);
        }

        try
        {
            var parameters = procedureParameters as DynamicParameters ?? new DynamicParameters(procedureParameters);

            var result = await executor(conn, tx!, parameters);

            if (ownConnection)
                await tx!.CommitAsync(ct);

            return result;
        }
        catch
        {
            if (ownConnection)
                await tx!.RollbackAsync(ct);
            throw;
        }
        finally
        {
            if (ownConnection && conn != null)
            {
                await conn.CloseAsync();
                await conn.DisposeAsync();
            }
        }
    }

    private static string BuildCallSqlNamedArgs(string procedureName, IEnumerable<string> parameterNames)
    {
        // p_cur necesita castearse a refcursor cuando se usa como argumento nombrado
        var args = parameterNames.Select(n =>
            n.Equals("p_cur", StringComparison.OrdinalIgnoreCase)
                ? $"p_cur => @p_cur::refcursor"
                : $"{n} => @{n}"
        );

        return $"CALL {procedureName}({string.Join(", ", args)});";
    }

    private static string[] GetParameterNames(DynamicParameters parameters, object? template)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Parámetros agregados manualmente
        foreach (var name in parameters.ParameterNames)
            names.Add(name);

        // Si es un objeto anónimo o clase, Dapper lo usa internamente pero no siempre lo expone en ParameterNames
        if (template != null && template is not DynamicParameters)
        {
            foreach (var prop in template.GetType().GetProperties())
                names.Add(prop.Name);
        }

        return names.ToArray();
    }

    private static string ResolveCursorName(string cursorName)
    {
        if (string.IsNullOrWhiteSpace(cursorName) || cursorName.Equals("result_cur", StringComparison.OrdinalIgnoreCase))
            return $"result_cur_{Guid.NewGuid():N}";

        EnsureSafeCursorName(cursorName);
        return cursorName;
    }

    private static string EnsureSafeCursorName(string cursorName)
    {
        if (string.IsNullOrWhiteSpace(cursorName))
            throw new ArgumentException("cursorName inválido.", nameof(cursorName));

        if (!Regex.IsMatch(cursorName, "^[A-Za-z0-9_]+$"))
            throw new ArgumentException("cursorName contiene caracteres inválidos.", nameof(cursorName));

        return cursorName;
    }
}