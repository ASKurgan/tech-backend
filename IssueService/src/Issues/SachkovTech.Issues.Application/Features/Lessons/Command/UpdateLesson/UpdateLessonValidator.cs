using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.UpdateLesson;

public class UpdateLessonValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonValidator()
    {
        RuleFor(a => a.Title)
            .MustBeValueObject(Title.Create);

        RuleFor(a => a.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(a => a.Experience)
            .MustBeValueObject(Experience.Create);
    }
}
