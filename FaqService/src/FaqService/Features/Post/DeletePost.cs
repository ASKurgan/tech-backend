using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FaqService.Features.Post;

public class DeletePost
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("posts/{id:guid}", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] SearchRepository searchRepository,
        [FromServices] ILogger<DeletePost> logger,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (post is null)
            return ResultResponse.BadRequest(Errors.General.NotFound(id));

        var answersOfPost = dbContext.Answers.Where(a => a.PostId == id).ToList();

        dbContext.Answers.RemoveRange(answersOfPost);

        dbContext.Posts.Remove(post);

        await dbContext.SaveChangesAsync(cancellationToken);
        await searchRepository.DeletePost(id, cancellationToken);

        logger.LogInformation($"Post {id} was deleted.");

        return ResultResponse.Ok(id);
    }
}