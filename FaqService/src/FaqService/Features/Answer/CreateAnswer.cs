using FaqService.Contracts;
using FaqService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FaqService.Features.Answer;

public class CreateAnswer
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("posts/{postId:guid}/answers", Handler);
        }
    }
    
    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [FromBody] CreateAnswerRequest request,
        [FromServices] ApplicationDbContext dbContext, 
        [FromServices] ILogger<CreateAnswer> logger,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post is null)
            return ResultResponse.NotFound(Errors.General.NotFound(postId));

        var answerResult = Entities.Answer.Create(
            postId,
            request.UserId,
            request.Text);

        if (answerResult.IsFailure)
            return ResultResponse.BadRequest(Errors.General.NotFound(postId));

        await dbContext.Answers.AddAsync(answerResult.Value, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"Created answer with id: {postId}");

        return ResultResponse.Ok(answerResult.Value.Id);
    }
}