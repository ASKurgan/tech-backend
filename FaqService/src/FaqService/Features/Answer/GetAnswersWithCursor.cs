using FaqService.Contracts;
using FaqService.Dtos;
using FaqService.Extensions;
using FaqService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework.Endpoints;

namespace FaqService.Features.Answer;

public class GetAnswersWithCursor
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("posts/{postId:guid}/answer/{answerId:guid}/cursor", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [FromRoute] Guid answerId,
        [AsParameters] GetAnswerQuery query,
        [FromServices] ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var answersQuery = dbContext.Answers
            .Where(p => p.Id == postId)
            .OrderByDescending(a => a.CreatedAt)
            .AsQueryable();

        var paginatedAnswers = await answersQuery.ToCursorList(query.Cursor, query.Limit, cancellationToken);

        var answerDtos = paginatedAnswers.Items.Select(a => new AnswerDto
        {
            Id = a.Id,
            IsSolution = a.IsSolution,
            PostId = a.PostId,
            Text = a.Text,
            UserId = a.UserId,
            Rating = a.Rating,
            CreatedAt = a.CreatedAt,
        }).ToList();

        return ResultResponse.Ok(new CursorList<AnswerDto>(
            items: answerDtos,
            cursor: paginatedAnswers.Cursor,
            nextCursor: paginatedAnswers.NextCursor,
            limit: paginatedAnswers.Limit,
            totalCount: paginatedAnswers.TotalCount
        ));
    }
}