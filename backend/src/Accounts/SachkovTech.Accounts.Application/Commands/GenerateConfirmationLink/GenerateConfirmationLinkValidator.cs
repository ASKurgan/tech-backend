using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.GenerateConfirmationLink;

public class GenerateConfirmationLinkValidator : AbstractValidator<GenerateConfirmationLinkCommand>
{
    public GenerateConfirmationLinkValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}