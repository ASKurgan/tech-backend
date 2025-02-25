using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework;
using SachkovTech.Framework.Authorization;
using SachkovTech.Issues.Application.Features.IssuesReviews.Commands.AddComment;
using SachkovTech.Issues.Application.Features.IssuesReviews.Commands.Approve;
using SachkovTech.Issues.Application.Features.IssuesReviews.Commands.DeleteComment;
using SachkovTech.Issues.Application.Features.IssuesReviews.Commands.SendForRevision;
using SachkovTech.Issues.Application.Features.IssuesReviews.Commands.StartReview;
using SachkovTech.Issues.Application.Features.IssuesReviews.Queries.GetCommentsWithPagination;
using SachkovTech.Issues.Contracts.IssueReview;
using SharedKernel;

namespace SachkovTech.Issues.Presentation.IssuesReviews;

public class IssuesReviewsController : ApplicationController
{
    [Permission(Permissions.IssuesReview.COMMENT_REVIEW_ISSUE)]
    [HttpPost("comment")]
    public async Task<ActionResult> Comment(
        [FromServices] AddCommentHandler handler,
        [FromRoute] Guid issueReviewId,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        string? userId = HttpContext.User.FindFirstValue(CustomClaims.ID);

        if (userId == null)
        {
            return Errors.Auth.InvalidCredentials().ToResponse();
        }

        var result = await handler.Handle(
            new AddCommentCommand(
                issueReviewId,
                Guid.Parse(userId),
                request.Message), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        return Ok(result.Value);
    }

    [Permission(Permissions.IssuesReview.CREATE_REVIEW_ISSUE)]
    [HttpPut("start-review")]
    public async Task<ActionResult> StartReview(
        [FromServices] StartReviewHandler handler,
        [FromRoute] Guid issueReviewId,
        CancellationToken cancellationToken)
    {
        string? userId = HttpContext.User.FindFirstValue(CustomClaims.ID);

        if (userId == null)
            return Errors.Auth.InvalidCredentials().ToResponse();

        var result = await handler.Handle(
            new StartReviewCommand(
                issueReviewId,
                Guid.Parse(userId)), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.IssuesReview.UPDATE_REVIEW_ISSUE)]
    [HttpPut("revision")]
    public async Task<ActionResult> SendForRevision(
        [FromServices] SendForRevisionHandler handler,
        [FromRoute] Guid issueReviewId,
        CancellationToken cancellationToken)
    {
        string? userId = HttpContext.User.FindFirstValue(CustomClaims.ID);

        if (userId == null)
            return Errors.Auth.InvalidCredentials().ToResponse();

        var result = await handler.Handle(
            new SendForRevisionCommand(issueReviewId, Guid.Parse(userId)), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.IssuesReview.UPDATE_REVIEW_ISSUE)]
    [HttpPut("approval")]
    public async Task<ActionResult> Approve(
        [FromServices] ApproveIssueReviewHandler handler,
        [FromRoute] Guid issueReviewId,
        CancellationToken cancellationToken)
    {
        string? userId = HttpContext.User.FindFirstValue(CustomClaims.ID);

        if (userId == null)
            return Errors.Auth.InvalidCredentials().ToResponse();

        var result = await handler.Handle(
            new ApproveIssueReviewCommand(issueReviewId, Guid.Parse(userId)), cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Permission(Permissions.IssuesReview.COMMENT_REVIEW_ISSUE)]
    [HttpDelete("comment/{commentId:guid}")]
    public async Task<ActionResult> DeleteComment(
        [FromServices] DeleteCommentHandler handler,
        [FromRoute] Guid issueReviewId,
        [FromRoute] Guid commentId,
        CancellationToken cancellationToken)
    {
        string? userId = HttpContext.User.FindFirstValue(CustomClaims.ID);

        if (userId == null)
            return Errors.Auth.InvalidCredentials().ToResponse();

        var result = await handler.Handle(
            new DeleteCommentCommand(
                issueReviewId,
                Guid.Parse(userId),
                commentId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}