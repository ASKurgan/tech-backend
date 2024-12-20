using ProjectTemplate.Domain;

namespace ProjectTemplate.Application.Managers;

public interface IPermissionManager
{
    Task<Permission?> FindByCode(string code);

    Task AddRangeIfExist(IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    Task<HashSet<string>> GetUserPermissionCodes(Guid userId, CancellationToken cancellationToken = default);
}