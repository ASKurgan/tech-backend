using FluentValidation;
using ProjectTemplate.Domain;
using SachkovTech.Core.Validation;

namespace ProjectTemplate.Application.Commands.UpdateUserSocialNetworks;

public class UpdateUserSocialNetworksCommandValidator : AbstractValidator<UpdateUserSocialNetworksCommand>
{
    public UpdateUserSocialNetworksCommandValidator()
    {
        RuleForEach(u => u.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));
    }
}