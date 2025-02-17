using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.VerifyConfirmationLink;

public class VerifyConfirmationLinkValidator : AbstractValidator<VerifyConfirmationLinkCommand>
{
    public VerifyConfirmationLinkValidator()
    {
        RuleFor(c => c.Code)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}