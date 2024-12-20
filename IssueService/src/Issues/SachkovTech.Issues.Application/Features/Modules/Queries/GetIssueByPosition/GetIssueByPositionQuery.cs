using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Application.Features.Modules.Queries.GetIssueByPosition;

public record GetIssueByPositionQuery(ModuleId ModuleId, int Position) : IQuery;