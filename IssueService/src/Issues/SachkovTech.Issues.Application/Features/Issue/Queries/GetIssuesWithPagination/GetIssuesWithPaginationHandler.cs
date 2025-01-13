using System.Data;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Issue;

namespace SachkovTech.Issues.Application.Features.Issue.Queries.GetIssuesWithPagination;

public class GetIssuesWithPaginationHandler
    : IQueryHandler<PagedList<IssueResponse>, GetFilteredIssuesWithPaginationQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetIssuesWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<IssueResponse>> Handle(
        GetFilteredIssuesWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var issuesQuery = _readDbContext.Issues;

        var totalCount = await issuesQuery.CountAsync(cancellationToken);

        Expression<Func<IssueDataModel, object>> keySelector = query.SortBy?.ToLower() switch
        {
            "title" => (issue) => issue.Title,
            _ => (issue) => issue.Id
        };

        issuesQuery = query.SortDirection?.ToLower() == "desc"
            ? issuesQuery.OrderByDescending(keySelector)
            : issuesQuery.OrderBy(keySelector);

        issuesQuery = issuesQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.Title),
            i => i.Title.Contains(query.Title!));

        var issues = issuesQuery.ToList()
            .Select(i => new IssueResponse
                {
                    Id = i.Id,
                    ModuleId = i.ModuleId,
                    LessonId = i.LessonId,
                    Title = i.Title,
                    Description = i.Description,
                });

        return new PagedList<IssueResponse>
        {
            Items = issues.ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
        };
    }
}

public class GetIssuesWithPaginationHandlerDapper
    : IQueryHandler<PagedList<IssueResponse>, GetFilteredIssuesWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<GetIssuesWithPaginationHandlerDapper> _logger;

    public GetIssuesWithPaginationHandlerDapper(ISqlConnectionFactory sqlConnectionFactory,
        ILogger<GetIssuesWithPaginationHandlerDapper> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task<PagedList<IssueResponse>> Handle(
        GetFilteredIssuesWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();

        var totalCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM issues.issues");

        var sql = new StringBuilder(
            """
              SELECT id, title, position, files FROM issues.issues
            """);

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            sql.Append(" WHERE title = @Title");
            parameters.Add("@Title", query.Title);
        }

        sql.ApplySorting(query.SortBy, query.SortDirection);
        sql.ApplyPagination(parameters, query.Page, query.PageSize);

        var issues = await connection.QueryAsync<IssueResponse>(
            sql.ToString(),
            param: parameters);

        return new PagedList<IssueResponse>
        {
            Items = issues.ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
        };
    }
}

public static class SqlExtensions
{
    public static void ApplySorting(
        this StringBuilder sqlBuilder,
        string? sortBy,
        string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy) || string.IsNullOrWhiteSpace(sortDirection)) return;

        var validSortDirections = new[] { "asc", "desc" };

        if (validSortDirections.Contains(sortDirection.ToLower()))
        {
            sqlBuilder.Append($"\norder by {sortBy} {sortDirection}");
        }
        else
        {
            throw new ArgumentException("Invalid sort parameters");
        }
    }

    public static void ApplyPagination(
        this StringBuilder sqlBuilder,
        DynamicParameters parameters,
        int page,
        int pageSize)
    {
        parameters.Add("@PageSize", pageSize, DbType.Int32);
        parameters.Add("@Offset", (page - 1) * pageSize, DbType.Int32);

        sqlBuilder.Append(" LIMIT @PageSize OFFSET @Offset");
    }
}