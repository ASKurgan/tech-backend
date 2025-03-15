using System.Data;
using Npgsql;
using SachkovTech.Core.Database;

namespace SachkovTech.Issues.IntegrationTests;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}