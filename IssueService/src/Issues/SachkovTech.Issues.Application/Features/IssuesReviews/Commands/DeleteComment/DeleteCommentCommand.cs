using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.DeleteComment;

public record DeleteCommentCommand(
    Guid IssueReviewId,
    Guid UserId,
    Guid CommentId) : ICommand;