using FluentValidation;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.Delete;

public class DeleteModuleCommandValidator : AbstractValidator<DeleteModuleCommand>
{
    public DeleteModuleCommandValidator()
    {
        RuleFor(d => d.ModuleId).NotEmpty();
    }
}