using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.VerifyConfirmationLink;

public class VerifyConfirmationLinkValidator : AbstractValidator<VerifyConfirmationLinkCommand>
{
    public VerifyConfirmationLinkValidator()
    {
        RuleFor(c => c.Code)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}