using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace ProjectTemplate.Application.Commands.UpdateUserName;

public class UpdateUserNameValidator : AbstractValidator<UpdateUserNameCommand>
{
    public UpdateUserNameValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("UserName"));
    }
}