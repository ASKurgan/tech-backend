using FluentValidation;
using ProjectTemplate.Domain;
using SachkovTech.Core.Validation;

namespace ProjectTemplate.Application.Commands.UpdateUserFullName;

public class UpdateUserFullNameCommandValidator : AbstractValidator<UpdateUserFullNameCommand>
{
    public UpdateUserFullNameCommandValidator()
    {
        RuleFor(u => u.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.SecondName, f.ThirdName));
    }
}