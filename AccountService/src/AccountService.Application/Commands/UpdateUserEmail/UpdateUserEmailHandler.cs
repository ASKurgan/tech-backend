using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Application.Commands.UpdateUserFullName;
using ProjectTemplate.Application.Database;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace ProjectTemplate.Application.Commands.UpdateUserEmail;

public class UpdateUserEmailHandler : ICommandHandler<Guid, UpdateUserEmailCommand>
{
    private readonly ILogger<UpdateUserFullNameHandler> _logger;
    private readonly IValidator<UpdateUserEmailCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // private readonly ICacheService _cache;
    public UpdateUserEmailHandler(
            ILogger<UpdateUserFullNameHandler> logger,
            IValidator<UpdateUserEmailCommand> validator,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)

        // ICacheService cache
    {
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;

        // _cache = cache;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateUserEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var userResult = await _userRepository.GetById(command.UserId, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        userResult.Value.UpdateEmail(command.Email);

        // var key = "users_" + userResult.Value.Id;
        //
        // var userCache = await _cache.GetAsync<UserDataModel>(key, cancellationToken);
        // if (userCache is not null)
        // {
        //     await _cache.RemoveAsync(key, cancellationToken);
        // }
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user main info successfully for {UserId}.", command.UserId);

        return userResult.Value.Id;
    }
}