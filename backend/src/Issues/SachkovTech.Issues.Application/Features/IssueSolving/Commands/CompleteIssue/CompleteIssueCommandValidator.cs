using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.CompleteIssue;

public class CompleteIssueCommandValidator : AbstractValidator<CompleteIssueCommand>
{
    public CompleteIssueCommandValidator()
    {
        RuleFor(c => c.UserIssueId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
    }
}
