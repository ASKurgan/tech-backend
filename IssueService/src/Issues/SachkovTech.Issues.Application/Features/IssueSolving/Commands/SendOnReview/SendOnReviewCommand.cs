using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.SendOnReview;

public record SendOnReviewCommand(Guid UserIssueId, Guid UserId, string PullRequestUrl) : ICommand;