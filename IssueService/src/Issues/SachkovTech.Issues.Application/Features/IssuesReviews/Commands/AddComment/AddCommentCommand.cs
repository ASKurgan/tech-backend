using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.AddComment;

public record AddCommentCommand(
    Guid IssueReviewId,
    Guid UserId,
    string Message) : ICommand;