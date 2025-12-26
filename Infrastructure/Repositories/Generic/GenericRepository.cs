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

    public async Task<T?> GetSingleByProcedureAsync<T>(
        string procedureName,
        object procedureParameters,
        string cursorName = "result_cur",
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
            // Construye los parámetros incluyendo el cursor
            var parameters = new DynamicParameters(procedureParameters);
            var propertyInfos = procedureParameters.GetType().GetProperties();
            foreach (var prop in propertyInfos)
            {
                parameters.Add(prop.Name, prop.GetValue(procedureParameters));
            }
            parameters.Add("p_cur", cursorName);

            var allParameterNames = propertyInfos.Select(p => p.Name).Concat(new[] { "p_cur" });
            var placeholders = allParameterNames.Select(n => n == "p_cur" ? "@p_cur::refcursor" : "@" + n);

            // Llama al procedure
            var callSql = $"CALL {procedureName}({string.Join(", ", placeholders)})";
            await connection.ExecuteAsync(callSql, parameters, tx);

            // Haz FETCH del cursor
            var fetchSql = $"FETCH ALL FROM {cursorName};";
            var result = await connection.QuerySingleOrDefaultAsync<T>(
                new CommandDefinition(fetchSql, transaction: tx)
            );

            if (ownConnection && tx != null)
                await tx.CommitAsync();

            return result;
        }
        catch
        {
            if (ownConnection && tx != null)
                await ((Npgsql.NpgsqlTransaction)tx).RollbackAsync();
            throw;
        }
        finally
        {
            if (ownConnection && connection != null)
                await connection.DisposeAsync();
        }
    }
}