using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateEmail;

public class UpdateUserEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
{
    public UpdateUserEmailCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Email"))
            .EmailAddress().WithError(Errors.General.ValueIsInvalid("Email"))
            .MaximumLength(256).WithError(Errors.General.ValueIsInvalid("Email"));
    }
}