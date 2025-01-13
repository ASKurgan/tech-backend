using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.Approve;

public class ApproveIssueReviewHandler : ICommandHandler<Guid, ApproveIssueReviewCommand>
{
    private readonly IIssuesReviewRepository _issuesReviewRepository;
    private readonly IUserIssueRepository _userIssueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ApproveIssueReviewCommand> _validator;
    private readonly ILogger<ApproveIssueReviewHandler> _logger;

    public ApproveIssueReviewHandler(
        IIssuesReviewRepository issuesReviewRepository,
        IUserIssueRepository userIssueRepository,
        IUnitOfWork unitOfWork,
        IValidator<ApproveIssueReviewCommand> validator,
        ILogger<ApproveIssueReviewHandler> logger)
    {
        _issuesReviewRepository = issuesReviewRepository;
        _userIssueRepository = userIssueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        ApproveIssueReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var issueReviewResult = await _issuesReviewRepository
            .GetById(IssueReviewId.Create(command.IssueReviewId), cancellationToken);

        if (issueReviewResult.IsFailure)
            return issueReviewResult.Error.ToErrorList();

        issueReviewResult.Value.Approve(UserId.Create(command.ReviewerId));

        var userIssueId = issueReviewResult.Value.UserIssueId;

        var sendIssueForRevisionRes = await ApproveIssue(userIssueId, cancellationToken);

        if (sendIssueForRevisionRes.IsFailure)
        {
            return sendIssueForRevisionRes.Error;
        }

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "IssueReview {issueReviewId} is approved",
            issueReviewResult.Value.Id.Value);

        _logger.LogInformation(
            "User Issue {userIssue} is completed",
            userIssueId.Value);

        return issueReviewResult.Value.Id.Value;
    }

    private async Task<Result<Guid, ErrorList>> ApproveIssue(
        Guid userIssueId, CancellationToken cancellationToken)
    {
        var userIssueResult = await _userIssueRepository
            .GetUserIssueById(UserIssueId.Create(userIssueId), cancellationToken);

        if (userIssueResult.IsFailure)
            return userIssueResult.Error.ToErrorList();

        var completeIssueResult = userIssueResult.Value.CompleteIssue();

        if (completeIssueResult.IsFailure)
            return completeIssueResult.Error.ToErrorList();

        return userIssueResult.Value.Id.Value;
    }
}