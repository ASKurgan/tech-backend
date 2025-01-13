using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.SendForRevision;

public class SendForRevisionHandler : ICommandHandler<Guid, SendForRevisionCommand>
{
    private readonly IIssuesReviewRepository _issuesReviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SendForRevisionCommand> _validator;
    private readonly ILogger<SendForRevisionHandler> _logger;
    private readonly IPublisher _publisher;

    public SendForRevisionHandler(
        IIssuesReviewRepository issuesReviewRepository,
        IUnitOfWork unitOfWork,
        IValidator<SendForRevisionCommand> validator,
        ILogger<SendForRevisionHandler> logger,
        IPublisher publisher)
    {
        _issuesReviewRepository = issuesReviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        SendForRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var issueReviewResult = await _issuesReviewRepository
            .GetById(IssueReviewId.Create(command.IssueReviewId), cancellationToken);

        if (issueReviewResult.IsFailure)
            return issueReviewResult.Error.ToErrorList();

        var issueReview = issueReviewResult.Value;

        issueReview.SendIssueForRevision(UserId.Create(command.ReviewerId));

        await _publisher.PublishDomainEvents(issueReview, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "IssueReview {issueReviewId} is sent for review",
            issueReview.Id.Value);

        return issueReview.Id.Value;
    }
}