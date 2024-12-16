using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.SendForRevision;

public class SendForRevisionCommandValidator : AbstractValidator<SendForRevisionCommand>
{
    public SendForRevisionCommandValidator()
    {
        RuleFor(c => c.IssueReviewId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
    }
}
