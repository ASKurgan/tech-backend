using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Modules.Queries.GetIssueByPosition;

public class GetIssueByPositionHandler : IQueryHandlerWithResult<Guid, GetIssueByPositionQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetIssueByPositionHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        GetIssueByPositionQuery query,
        CancellationToken cancellationToken = default)
    {
        var module = await _readDbContext.Modules
            .FirstOrDefaultAsync(i => i.Id == query.ModuleId, cancellationToken);

        if (module is null)
            return Errors.General.NotFound().ToErrorList();

        var issueDto = module.IssuesPosition
            .FirstOrDefault(i => i.Position == query.Position);

        if (issueDto is null)
            return Errors.General.NotFound().ToErrorList();

        return issueDto.IssueId;
    }
}