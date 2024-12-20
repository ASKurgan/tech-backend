namespace SachkovTech.Core.Database;

public interface IMigrator
{
    Task Migrate(CancellationToken cancellationToken = default);
}
