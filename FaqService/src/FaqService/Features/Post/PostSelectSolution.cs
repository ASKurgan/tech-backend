using FaqService.Extensions;
using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FaqService.Features.Post;

public class PostSelectSolution
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("posts/{postId:guid}/select-solution/{answerId:guid}/answer", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [FromRoute] Guid answerId,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] SearchRepository searchRepository,
        [FromServices] ElasticIndexRecoveryService indexRecoveryService,
        [FromServices] UnitOfWork unitOfWork,
        [FromServices] ILogger<PostSelectSolution> logger,
        CancellationToken cancellationToken)
    {
        var transaction = await unitOfWork.BeginTransaction(cancellationToken);
        bool indexResult = false;

        try
        {
            var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId, cancellationToken);
            if (post is null)
                return ResultResponse.BadRequest(Errors.General.NotFound(postId));

            var answer = await dbContext.Answers
                .SingleOrDefaultAsync(a => a.Id == answerId && a.PostId == postId, cancellationToken);
            if (answer is null)
                return ResultResponse.BadRequest(Errors.General.NotFound(answerId));

            answer.ChangeIsSolution(true);

            await dbContext.SaveChangesAsync(cancellationToken);
            indexResult = await searchRepository.IndexPost(post);

            transaction.Commit();

            logger.LogInformation($"For post {postId} selected solution {answerId}.");

            return ResultResponse.Ok(post.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cannot update post in transaction");

            transaction.Rollback();

            if (indexResult)
                await searchRepository.DeletePost(postId, cancellationToken);

            await indexRecoveryService.RestoreElasticIndex(postId, indexResult, cancellationToken);

            return ResultResponse.BadRequest(
                Error.Failure("Cannot update post in transaction", "post.update.failure"));
        }
    }
}