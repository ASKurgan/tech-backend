using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Domain.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.UpdateMainInfo;

public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoCommand>
{
    public UpdateMainInfoCommandValidator()
    {
        RuleFor(r => r.ModuleId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(r => r.Title).MustBeValueObject(Title.Create);
        RuleFor(r => r.Description).MustBeValueObject(Description.Create);
    }
}