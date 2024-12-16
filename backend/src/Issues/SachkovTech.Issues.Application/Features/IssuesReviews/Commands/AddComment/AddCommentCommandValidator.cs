using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Domain.IssuesReviews.ValueObjects;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(c => c.IssueReviewId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
        RuleFor(c => c.Message).MustBeValueObject(Message.Create);
    }
}