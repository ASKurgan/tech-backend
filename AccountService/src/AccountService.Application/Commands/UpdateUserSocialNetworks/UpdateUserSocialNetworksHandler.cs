using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Application.Database;
using ProjectTemplate.Domain;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace ProjectTemplate.Application.Commands.UpdateUserSocialNetworks;

public class UpdateUserSocialNetworksHandler : ICommandHandler<Guid, UpdateUserSocialNetworksCommand>
{
    private readonly IValidator<UpdateUserSocialNetworksCommand> _validator;
    private readonly IUserRepository _repository;
    private readonly ILogger<UpdateUserSocialNetworksHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    // private readonly ICacheService _cache;
    public UpdateUserSocialNetworksHandler(
        IValidator<UpdateUserSocialNetworksCommand> validator,
        IUserRepository repository,
        ILogger<UpdateUserSocialNetworksHandler> logger,
        IUnitOfWork unitOfWork)

        // ICacheService cache
    {
        _validator = validator;
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;

        // _cache = cache;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateUserSocialNetworksCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var userResult = await _repository.GetById(command.UserId, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        var socialNetworks = command.SocialNetworks
            .Select(s => SocialNetwork.Create(s.Name, s.Link).Value);

        userResult.Value.UpdateSocialNetworks(socialNetworks);

        // var key = "users_" + userResult.Value.Id;
        //
        // var userCache = await _cache.GetAsync<UserDataModel>(key, cancellationToken);
        // if (userCache is not null)
        // {
        //     await _cache.RemoveAsync(key, cancellationToken);
        // }
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user social networks successfully for {UserId}.", command.UserId);

        return userResult.Value.Id;
    }
}