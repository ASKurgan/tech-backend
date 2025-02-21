using FaqService.Dtos;
using FaqService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Framework.Endpoints;

namespace FaqService.Features.Answer;

public class GetAnswerAtPostById
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("posts/{postId:guid}/answer/{answerId:guid}", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [FromRoute] Guid answerId,
        [FromServices] ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var answer = await dbContext.Answers
            .SingleOrDefaultAsync(a => a.Id == answerId && a.PostId == postId, cancellationToken);

        return ResultResponse.Ok(answer is not null
            ? new AnswerDto
            {
                Id = answer.Id,
                IsSolution = answer.IsSolution,
                PostId = answer.PostId,
                Text = answer.Text,
                UserId = answer.UserId,
                Rating = answer.Rating,
                CreatedAt = answer.CreatedAt,
            }
            : null);
    }
}