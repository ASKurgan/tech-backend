using FluentValidation;
using SachkovTech.Core.Validation;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForLesson;

public class UpdateLessonAchievementConditionCommandValidator : AbstractValidator<UpdateLessonAchievementConditionCommand>
{
    public UpdateLessonAchievementConditionCommandValidator()
    {
        RuleFor(c => c.LessonCondition.Difficulty)
            .MustBeValueObject(Difficulty.Create);

        RuleFor(c => c.LessonCondition).MustBeValueObject(l =>
            LessonCondition.Create(
                l.TimeToComplete,
                Difficulty.Create(l.Difficulty).Value,
                l.RequiredCount));
    }
}