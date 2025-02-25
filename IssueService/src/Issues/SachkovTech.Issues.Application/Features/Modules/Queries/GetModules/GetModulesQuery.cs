using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Modules.Queries.GetModules;

public record GetModulesQuery(string? Title, int Page, int PageSize) : IQuery;