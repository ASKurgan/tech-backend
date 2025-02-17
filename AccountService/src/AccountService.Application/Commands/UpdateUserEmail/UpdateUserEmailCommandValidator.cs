using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateUserEmail;

public class UpdateUserEmailCommandValidator : AbstractValidator<UpdateUserEmailCommand>
{
    public UpdateUserEmailCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Email"))
            .EmailAddress().WithError(Errors.General.ValueIsInvalid("Email"))
            .MaximumLength(256).WithError(Errors.General.ValueIsInvalid("Email"));
    }
}