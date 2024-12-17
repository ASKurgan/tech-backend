using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Issue;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Queries.GetIssueById;

public class GetIssueByIdHandler : IQueryHandlerWithResult<IssueResponse, GetIssueByIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetIssueByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IssueResponse, ErrorList>> Handle(
        GetIssueByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var issueDto = await _readDbContext.Issues
            .SingleOrDefaultAsync(i => i.Id == query.IssueId, cancellationToken);

        if (issueDto is null)
            return Errors.General.NotFound(query.IssueId).ToErrorList();

        var response = new IssueResponse
        {
            Id = issueDto.Id,
            ModuleId = issueDto.ModuleId,
            Title = issueDto.Title,
            Description = issueDto.Description,
            LessonId = issueDto.LessonId,
        };

        return response;
    }
}