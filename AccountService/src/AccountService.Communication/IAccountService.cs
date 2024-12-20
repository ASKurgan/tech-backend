using AccountService.Contracts.Responses;
using CSharpFunctionalExtensions;

namespace AccountService.Communication;

public interface IAccountService
{
    Task<Result<ConfirmationLinkResponse, string>> GetConfirmationLink(
        Guid userId,
        CancellationToken cancellationToken = default);
}