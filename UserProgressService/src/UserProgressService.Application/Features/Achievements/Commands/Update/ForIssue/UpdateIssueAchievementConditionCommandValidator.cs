using FluentValidation;
using SachkovTech.Core.Validation;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForIssue;

public class UpdateIssueAchievementConditionCommandValidator : AbstractValidator<UpdateIssueAchievementConditionCommand>
{
    public UpdateIssueAchievementConditionCommandValidator()
    {
        RuleFor(c => c.IssueCondition.Difficulty)
            .MustBeValueObject(Difficulty.Create);

        RuleFor(c => c.IssueCondition).MustBeValueObject(l =>
            IssueCondition.Create(
                l.TimeToComplete,
                Difficulty.Create(l.Difficulty).Value,
                l.Attempts,
                l.IssueCount));
    }
}