using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery;