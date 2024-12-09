using Microsoft.EntityFrameworkCore;

namespace FileService.Jobs;

public class HangfireContext : DbContext
{
    private readonly string _connectionString;

    public HangfireContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}