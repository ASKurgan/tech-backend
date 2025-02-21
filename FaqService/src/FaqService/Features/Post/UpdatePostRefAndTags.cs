using FaqService.Contracts;
using FaqService.Extensions;
using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FaqService.Features.Post;

public class UpdatePostRefAndTags
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("posts/{postId:guid}/refs-and-tags", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [FromBody] UpdatePostRefAndTagsRequest request,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] SearchRepository searchRepository,
        [FromServices] ElasticIndexRecoveryService indexRecoveryService,
        [FromServices] UnitOfWork unitOfWork,
        [FromServices] ILogger<UpdatePostRefAndTags> logger,
        CancellationToken cancellationToken)
    {
        var transaction = await unitOfWork.BeginTransaction(cancellationToken);
        var indexResult = false;
        try
        {
            var post = await dbContext.Posts
                .SingleOrDefaultAsync(p => p.Id == postId, cancellationToken);

            if (post is null)
                return ResultResponse.BadRequest(Errors.General.NotFound(postId));

            post.UpdateRefsAndTags(request.ReplLink, request.IssueId, request.LessonId, request.Tags);

            await dbContext.SaveChangesAsync(cancellationToken);
            indexResult = await searchRepository.IndexPost(post);

            transaction.Commit();

            logger.LogInformation($"Updated refs and tags post {postId}.");

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