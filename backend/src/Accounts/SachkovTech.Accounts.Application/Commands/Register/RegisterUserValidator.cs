using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
        
        RuleFor(c => c.UserName)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
        
        RuleFor(c => c.Password)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}