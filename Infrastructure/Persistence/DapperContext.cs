using System.Data;
using Npgsql;

namespace MenuSoda.Infrastructure.Persistence;

public class DapperContext
{
    private readonly string _connectionString;
    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("URL_SERVER") 
            ?? throw new Exception("Cadena de conexión a base de datos no encontrada.");
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}