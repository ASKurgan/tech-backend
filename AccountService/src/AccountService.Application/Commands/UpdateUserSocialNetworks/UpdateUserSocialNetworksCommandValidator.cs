using AccountService.Domain;
using FluentValidation;
using SachkovTech.Core.Validation;

namespace AccountService.Application.Commands.UpdateUserSocialNetworks;

public class UpdateUserSocialNetworksCommandValidator : AbstractValidator<UpdateUserSocialNetworksCommand>
{
    public UpdateUserSocialNetworksCommandValidator()
    {
        RuleForEach(u => u.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));
    }
}