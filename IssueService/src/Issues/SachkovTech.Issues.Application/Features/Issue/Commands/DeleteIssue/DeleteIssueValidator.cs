using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue;

public class DeleteIssueValidator : AbstractValidator<DeleteIssueCommand>
{
    public DeleteIssueValidator()
    {
        RuleFor(u => u.IssueId).NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}