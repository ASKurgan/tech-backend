using AccountService.Application.Database;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateEmail;

public class UpdateEmailHandler : ICommandHandler<Guid, UpdateEmailCommand>
{
    private readonly ILogger<UpdateEmailHandler> _logger;
    private readonly IValidator<UpdateEmailCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // private readonly ICacheService _cache;
    public UpdateEmailHandler(
            ILogger<UpdateEmailHandler> logger,
            IValidator<UpdateEmailCommand> validator,
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
        UpdateEmailCommand command,
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