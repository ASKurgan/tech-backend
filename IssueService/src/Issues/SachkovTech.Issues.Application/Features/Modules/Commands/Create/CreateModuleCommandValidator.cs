using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Domain.ValueObjects;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.Create;

public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(c => c.Title).MustBeValueObject(Title.Create);
        RuleFor(c => c.Description).MustBeValueObject(Description.Create);
    }
}