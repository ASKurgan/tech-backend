using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.SendOnReview;

public class SendOnReviewHandler : ICommandHandler<SendOnReviewCommand>
{
    private readonly IUserIssueRepository _userIssueRepository;
    private readonly ILogger<SendOnReviewHandler> _logger;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SendOnReviewCommand> _validator;

    public SendOnReviewHandler(
        IUserIssueRepository userIssueRepository,
        ILogger<SendOnReviewHandler> logger,
        [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
        IValidator<SendOnReviewCommand> validator,
        IOutboxRepository outboxRepository, IPublisher publisher)
    {
        _logger = logger;
        _userIssueRepository = userIssueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _outboxRepository = outboxRepository;
        _publisher = publisher;
    }

    public async Task<UnitResult<ErrorList>> Handle(SendOnReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (validationResult.IsValid == false)
        {
            return validationResult.ToList();
        }

        var (_, isFailure, userIssue, error) = await _userIssueRepository
            .GetUserIssueById(UserIssueId.Create(command.UserIssueId), cancellationToken);

        if (isFailure)
        {
            _logger.LogError("UserIssue with {Id} not found", command.UserIssueId);
            return error.ToErrorList();
        }

        var pullRequestUrl = PullRequestUrl.Create(command.PullRequestUrl).Value;

        var sendOnReviewResult = userIssue.SendOnReview(pullRequestUrl);

        if (sendOnReviewResult.IsFailure)
        {
            return sendOnReviewResult.Error.ToErrorList();
        }

        await _publisher.PublishDomainEvents(userIssue, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Issue with UserIssueId {UserIssueId} was created", command.UserIssueId);

        return UnitResult.Success<ErrorList>();
    }
}