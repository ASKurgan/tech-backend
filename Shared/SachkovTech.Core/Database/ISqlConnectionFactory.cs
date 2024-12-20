using System.Data;

namespace SachkovTech.Core.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}