using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.UpdateLessonPosition;

public class UpdateLessonPositionCommandValidator : AbstractValidator<UpdateLessonPositionCommand>
{
    public UpdateLessonPositionCommandValidator()
    {
        RuleFor(a => a.LessonId)
            .NotNull().WithError(Errors.General.ValueIsRequired("LessonId"));

        RuleFor(a => a.ModuleId)
            .NotNull().WithError(Errors.General.ValueIsRequired("ModuleId"));

        RuleFor(a => a.Position)
            .NotNull().WithError(Errors.General.ValueIsRequired("Position"));
    }
}