using System.Data;
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
        string cursorName = "result_cur",
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<T>(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                // Añadir cursor
                parameters.Add("p_cur", cursorName);

                var propertyInfos = procedureParameters.GetType().GetProperties();
                var allParameterNames = propertyInfos.Select(p => p.Name).Concat(new[] { "p_cur" });
                var placeholders = allParameterNames.Select(n => n == "p_cur" ? "@p_cur::refcursor" : "@" + n);

                var callSql = $"CALL {procedureName}({string.Join(", ", placeholders)})";
                await conn.ExecuteAsync(callSql, parameters, tx);

                var fetchSql = $"FETCH ALL FROM {cursorName};";
                return await conn.QuerySingleOrDefaultAsync<T>(
                    new CommandDefinition(fetchSql, transaction: tx)
                );
            },
            transaction
        );
    }

    public Task<IEnumerable<T>?> GetListByProcedureAsync<T>(
    string procedureName,
    object procedureParameters,
    string cursorName = "result_cur",
    NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<IEnumerable<T>>(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                // Añadir cursor
                parameters.Add("p_cur", cursorName);

                var propertyInfos = procedureParameters.GetType().GetProperties();
                var allParameterNames = propertyInfos.Select(p => p.Name).Concat(new[] { "p_cur" });
                var placeholders = allParameterNames.Select(n => n == "p_cur" ? "@p_cur::refcursor" : "@" + n);

                // Llamada al procedure
                var callSql = $"CALL {procedureName}({string.Join(", ", placeholders)})";
                await conn.ExecuteAsync(callSql, parameters, tx);

                // Fetch del cursor → devuelve varias filas
                var fetchSql = $"FETCH ALL FROM {cursorName};";
                var result = await conn.QueryAsync<T>(
                    new CommandDefinition(fetchSql, transaction: tx)
                );

                return result;
            },
            transaction
        );
    }

    public Task<T?> ExecuteProcedureWithOutputsAsync<T>(
        string procedureName,
        object procedureParameters,
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<T>(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                var propertyInfos = procedureParameters.GetType().GetProperties();
                var placeholders = propertyInfos.Select(p => "@" + p.Name);
                var callSql = $"CALL {procedureName}({string.Join(", ", placeholders)})";

                // Como hay múltiples OUT, usamos QuerySingleOrDefaultAsync<T>
                var result = await conn.QuerySingleOrDefaultAsync<T>(
                    new CommandDefinition(callSql, parameters, transaction: tx)
                );

                return result;
            },
            transaction
        );
    }

    public Task<int> ExecuteNonQueryProcedureAsync(
        string procedureName,
        object procedureParameters,
        NpgsqlTransaction? transaction = null)
    {
        return ExecuteProcedureAsync<int>(
            procedureParameters,
            async (conn, tx, parameters) =>
            {
                var propertyInfos = procedureParameters.GetType().GetProperties();
                var placeholders = propertyInfos.Select(p => "@" + p.Name);
                var callSql = $"CALL {procedureName}({string.Join(", ", placeholders)})";

                // Ejecuta y devuelve filas afectadas
                var affectedRows = await conn.ExecuteAsync(
                    new CommandDefinition(callSql, parameters, transaction: tx)
                );

                return affectedRows;
            },
            transaction
        );
    }

    private async Task<T?> ExecuteProcedureAsync<T>(
        object procedureParameters,
        Func<NpgsqlConnection, NpgsqlTransaction, DynamicParameters, Task<T?>> executor,
        NpgsqlTransaction? transaction = null)
    {
        bool ownConnection = false;
        NpgsqlConnection? connection = null;
        NpgsqlTransaction? tx = transaction;

        if (transaction == null)
        {
            connection = (NpgsqlConnection?)_context.CreateConnection();
            if (connection is null)
                throw new InvalidOperationException("Fallo al crear la conexión a la base de datos.");
            await connection.OpenAsync();
            tx = await connection.BeginTransactionAsync();
            ownConnection = true;
        }
        else
        {
            connection = transaction.Connection!;
        }

        try
        {
            // Construye parámetros dinámicos
            var parameters = new DynamicParameters(procedureParameters);
            var propertyInfos = procedureParameters.GetType().GetProperties();
            foreach (var prop in propertyInfos)
            {
                parameters.Add(prop.Name, prop.GetValue(procedureParameters));
            }

            // Ejecuta la parte variable
            var result = await executor(connection!, tx!, parameters);

            if (ownConnection && tx != null)
                await tx.CommitAsync();

            return result;
        }
        catch
        {
            if (ownConnection && tx != null)
                await tx.RollbackAsync();
            throw;
        }
        finally
        {
            if (ownConnection && connection != null)
                await connection.DisposeAsync();
        }
    }
}