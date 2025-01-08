using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.MainInfo;

public class UpdateMainInfoAchievementCommandValidator : AbstractValidator<UpdateMainInfoAchievementCommand>
{
    public UpdateMainInfoAchievementCommandValidator()
    {
        RuleFor(c => c.IconId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("IconId"));

        RuleFor(c => c.Name)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Name"));

        RuleFor(c => c.Description)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Description"));

        RuleFor(c => c.Experience)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Experience"));
    }
}