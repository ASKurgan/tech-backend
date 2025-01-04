using MassTransit;
using SachkovTech.Issues.Contracts.Messaging;

namespace NotificationService.Consumers;

public class UserSentIssueOnReviewConsumer : IConsumer<IssueSentOnReviewEvent>
{
    private readonly ILogger<UserSentIssueOnReviewConsumer> _logger;
    public UserSentIssueOnReviewConsumer(ILogger<UserSentIssueOnReviewConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IssueSentOnReviewEvent> context)
    {
        return Task.CompletedTask;
    }
}