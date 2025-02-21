using FaqService.Contracts;
using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FaqService.Features.Post;

public class CreatePost
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("posts", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromBody] CreatePostRequest request,
        [FromServices] UnitOfWork unitOfWork,
        [FromServices] SearchRepository searchRepository,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] ILogger<CreatePost> logger,
        CancellationToken cancellationToken)
    {
        var transaction = await unitOfWork.BeginTransaction(cancellationToken);
        var id = Guid.NewGuid();

        try
        {
            var postResult = Entities.Post.Create(
                id,
                request.Title,
                request.Description,
                request.ReplLink,
                request.UserId,
                request.IssueId,
                request.LessonId,
                request.Tags);

            if (postResult.IsFailure)
                return ResultResponse.BadRequest(postResult.Error);

            await dbContext.Posts.AddAsync(postResult.Value, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await searchRepository.IndexPost(postResult.Value);

            transaction.Commit();

            logger.LogInformation("Created post {PostId}.", postResult.Value);

            return ResultResponse.Ok(postResult.Value.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Cannot create post in transaction");

            transaction.Rollback();

            await searchRepository.DeletePost(id, cancellationToken);

            return ResultResponse.BadRequest(
                Error.Failure("Cannot create post in transaction", "post.create.failure"));
        }
    }
}