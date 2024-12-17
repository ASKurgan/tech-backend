using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.Approve;

public class ApproveIssueReviewCommandValidator : AbstractValidator<ApproveIssueReviewCommand>
{
    public ApproveIssueReviewCommandValidator()
    {
        RuleFor(c => c.IssueReviewId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
    }
}
