using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.StartReview;

public class StartReviewCommandValidator : AbstractValidator<StartReviewCommand>
{
    public StartReviewCommandValidator()
    {
        RuleFor(c => c.IssueReviewId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
    }
}
