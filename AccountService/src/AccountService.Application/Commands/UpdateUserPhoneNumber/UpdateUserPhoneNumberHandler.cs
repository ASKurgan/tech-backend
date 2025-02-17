using AccountService.Application.Commands.UpdateUserFullName;
using AccountService.Application.Database;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateUserPhoneNumber;

public class UpdateUserPhoneNumberHandler : ICommandHandler<Guid, UpdateUserPhoneNumberCommand>
{
    private readonly ILogger<UpdateUserFullNameHandler> _logger;
    private readonly IValidator<UpdateUserPhoneNumberCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // private readonly ICacheService _cache;
    public UpdateUserPhoneNumberHandler(
            ILogger<UpdateUserFullNameHandler> logger,
            IValidator<UpdateUserPhoneNumberCommand> validator,
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
        UpdateUserPhoneNumberCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var userResult = await _userRepository.GetById(command.UserId, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        if (command.PhoneNumber != null)
        {
            var userPhoneNumberResult = await _userRepository.GetByPhoneNumber(command.PhoneNumber, cancellationToken);

            if (userPhoneNumberResult != null && userResult.Value.Id != userPhoneNumberResult.Id)
                return Errors.General.AlreadyExist().ToErrorList();
        }

        userResult.Value.UpdatePhoneNumber(command.PhoneNumber);

        // var key = "users_" + userResult.Value.Id;
        //
        // var userCache = await _cache.GetAsync<UserDataModel>(key, cancellationToken);
        // if (userCache is not null)
        // {
        //     await _cache.RemoveAsync(key, cancellationToken);
        // }
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user phone number successfully for {UserId}.", command.UserId);

        return userResult.Value.Id;
    }
}