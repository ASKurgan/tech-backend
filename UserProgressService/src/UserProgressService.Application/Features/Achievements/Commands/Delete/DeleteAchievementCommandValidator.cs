using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace UserProgressService.Application.Features.Achievements.Commands.Delete;

public class DeleteAchievementCommandValidator : AbstractValidator<DeleteAchievementCommand>
{
    public DeleteAchievementCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Id"));
    }
}