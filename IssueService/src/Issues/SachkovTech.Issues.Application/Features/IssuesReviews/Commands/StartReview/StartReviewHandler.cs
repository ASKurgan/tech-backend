using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.StartReview;

public class StartReviewHandler : ICommandHandler<Guid, StartReviewCommand>
{
    private readonly IIssuesReviewRepository _issuesReviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<StartReviewCommand> _validator;
    private readonly ILogger<StartReviewHandler> _logger;

    public StartReviewHandler(
        IIssuesReviewRepository issuesReviewRepository,
        IUnitOfWork unitOfWork,
        IValidator<StartReviewCommand> validator,
        ILogger<StartReviewHandler> logger)
    {
        _issuesReviewRepository = issuesReviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        StartReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var issueReviewResult = await _issuesReviewRepository
            .GetById(IssueReviewId.Create(command.IssueReviewId), cancellationToken);

        if (issueReviewResult.IsFailure)
            return issueReviewResult.Error.ToErrorList();

        issueReviewResult.Value.StartReview(UserId.Create(command.ReviewerId));
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "IssueReview {issueReviewId} started by user {userId}",
            issueReviewResult.Value.Id.Value,
            issueReviewResult.Value.UserId);

        return issueReviewResult.Value.Id.Value;
    }
}