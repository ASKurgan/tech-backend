using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery;