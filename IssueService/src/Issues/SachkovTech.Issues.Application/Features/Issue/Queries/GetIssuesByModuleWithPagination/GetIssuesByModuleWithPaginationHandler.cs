using System.Text;
using CSharpFunctionalExtensions;
using Dapper;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Features.Issue.Queries.GetIssuesWithPagination;
using SachkovTech.Issues.Contracts.Issue;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Queries.GetIssuesByModuleWithPagination;

public class GetIssuesByModuleWithPaginationHandler
    : IQueryHandlerWithResult<PagedList<IssueResponse>, GetFilteredIssuesByModuleWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetIssuesByModuleWithPaginationHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PagedList<IssueResponse>, ErrorList>> Handle(
        GetFilteredIssuesByModuleWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        using var connection = _sqlConnectionFactory.Create();
        
        var parameters = new DynamicParameters();

        var sqlBuilder = new StringBuilder(
            """
            SELECT
                i.id AS Id,
                i.lesson_id AS LessonId,
                i.module_id AS ModuleId,
                (ip->>'Position')::int AS Position,
                i.files AS Files,
                i.is_deleted AS IsDeleted,
                i.description AS Description,
                i.title AS Title
            FROM issues.issues AS i
                     JOIN issues.modules AS m
                          ON i.module_id = m.id
                     JOIN LATERAL jsonb_array_elements(m.issues_position) AS ip ON (ip->>'IssueId')::uuid = i.id
            WHERE NOT i.is_deleted
            
            """);

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            sqlBuilder.Append("\nAND i.title ILIKE @Title");
            parameters.Add("@Title", $"%{query.Title}%");
        }

        sqlBuilder.ApplySorting(query.SortBy, query.SortDirection);
        sqlBuilder.ApplyPagination(parameters, query.Page, query.PageSize);

        var totalCountSql = new StringBuilder(
            """
            SELECT COUNT(*)
            FROM issues.issues AS i
            JOIN issues.modules AS m ON i.module_id = m.id
            WHERE NOT i.is_deleted
            """);

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            totalCountSql.Append("\nAND i.title ILIKE @Title");
        }
        
        var totalCount = await connection.ExecuteScalarAsync<long>(
            totalCountSql.ToString(),
            parameters);

        var issues = await connection.QueryAsync<IssueResponse>(
            sqlBuilder.ToString(),
            param: parameters);

        return new PagedList<IssueResponse>
        {
            Items = issues.ToList(),
            TotalCount = totalCount,
            PageSize = query.PageSize,
            Page = query.Page
        };
    }
}