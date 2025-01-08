using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;

namespace UserProgressService.Application.Features.Achievements.Commands.Create.ForLesson;

public class CreateLessonAchievementCommandValidator : AbstractValidator<CreateLessonAchievementCommand>
{
    public CreateLessonAchievementCommandValidator()
    {
        RuleFor(c => c.IconId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("IconId"));

        RuleFor(c => c.Name)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Name"));

        RuleFor(c => c.Description)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Description"));

        RuleFor(c => c.Experience)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Experience"));

        RuleFor(c => c.LessonCondition.Difficulty)
            .MustBeValueObject(Difficulty.Create);

        RuleFor(c => c.LessonCondition).MustBeValueObject(l =>
            LessonCondition.Create(
                l.TimeToComplete,
                Difficulty.Create(l.Difficulty).Value,
                l.RequiredCount));
    }
}