using FluentValidation;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace ProjectTemplate.Application.Commands.GenerateConfirmationLink;

public class GenerateConfirmationLinkValidator : AbstractValidator<GenerateConfirmationLinkCommand>
{
    public GenerateConfirmationLinkValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}