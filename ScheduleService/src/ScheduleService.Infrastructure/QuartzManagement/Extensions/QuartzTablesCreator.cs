using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ScheduleService.Infrastructure.Database;

namespace ScheduleService.Infrastructure.QuartzManagement.Extensions;
public static class QuartzTablesCreator
{
    private const string SQL_SCRIPT_PATH =
        @"//ScheduleService.Infrastructure//Database//DatabaseScripts//CreateQuartzDatabase.sql";

    /// <summary>
    ///    Метод расширения создает служебные таблицы для работы Quartz на основе SQL скрипта.
    /// </summary>    
    public static IServiceCollection CreateQuartzTables(this
        IServiceCollection services, IConfiguration configuration)
    {
        var scriptPath = $"{Directory.GetParent(Directory.GetCurrentDirectory())}{SQL_SCRIPT_PATH}";
        var script = File.ReadAllText(scriptPath);

        var connectionString = configuration.GetConnectionString(ApplicationDbContext.DATABASE);

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var dbCommand = connection.CreateCommand();

        dbCommand.CommandText = script;

        dbCommand.ExecuteNonQuery();

        return services;
    }
}
