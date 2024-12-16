using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssuesReviews.Commands.DeleteComment;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(c => c.IssueReviewId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
        RuleFor(c => c.CommentId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("comment_id"));
    }
}