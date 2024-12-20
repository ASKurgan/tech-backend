using MediatR;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.IssuesReviews.Events;

namespace SachkovTech.Issues.Application.EventHandlers.IssueSentForRevisionEventHandlers;

public class UserIssueSendForRevision : INotificationHandler<IssueSentForRevisionEvent>
{
    private readonly IUserIssueRepository _userIssueRepository;
    
    public UserIssueSendForRevision(IUserIssueRepository userIssueRepository)
    {
        _userIssueRepository = userIssueRepository;

    }

    public async Task Handle(IssueSentForRevisionEvent domainEvent, CancellationToken cancellationToken)
    {
        var userIssueResult = await _userIssueRepository
            .GetUserIssueById(domainEvent.UserIssueId, cancellationToken);

        if (userIssueResult.IsFailure)
        {
            throw new Exception(userIssueResult.Error.Message);
        }

        var userIssue = userIssueResult.Value;

        var sendForRevisionResult = userIssue.SendForRevision();
        
        if (sendForRevisionResult.IsFailure)
        {
            throw new Exception(sendForRevisionResult.Error.Message);
        }
    }
}