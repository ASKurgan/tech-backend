using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateUserPhoneNumber;

public class UpdateUserPhoneNumberCommandValidator : AbstractValidator<UpdateUserPhoneNumberCommand>
{
    private const string PhoneNumberPattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";

    public UpdateUserPhoneNumberCommandValidator()
    {
        RuleFor(u => u.PhoneNumber)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("PhoneNumber"))
            .Matches(PhoneNumberPattern).WithError(Errors.General.ValueIsInvalid("PhoneNumber"))
            .MaximumLength(15).WithError(Errors.General.ValueIsInvalid("PhoneNumber"));
    }
}