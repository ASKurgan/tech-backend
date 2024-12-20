using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.SendForRevision;

public class SendUserIssueForRevisionCommandValidator : AbstractValidator<SendUserIssueForRevisionCommand>
{
    public SendUserIssueForRevisionCommandValidator()
    {
        RuleFor(c => c.UserIssueId)
            .NotEmpty().WithError(Errors.General.ValueIsInvalid("id"));
    }
}
