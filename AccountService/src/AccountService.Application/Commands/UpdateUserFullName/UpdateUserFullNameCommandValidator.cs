using AccountService.Domain;
using FluentValidation;
using SachkovTech.Core.Validation;

namespace AccountService.Application.Commands.UpdateUserFullName;

public class UpdateUserFullNameCommandValidator : AbstractValidator<UpdateUserFullNameCommand>
{
    public UpdateUserFullNameCommandValidator()
    {
        RuleFor(u => u.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.SecondName, f.ThirdName));
    }
}