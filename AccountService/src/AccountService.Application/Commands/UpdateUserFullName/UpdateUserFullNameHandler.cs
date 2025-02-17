using AccountService.Application.Database;
using AccountService.Domain;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateUserFullName;

public class UpdateUserFullNameHandler : ICommandHandler<Guid, UpdateUserFullNameCommand>
{
    private readonly ILogger<UpdateUserFullNameHandler> _logger;
    private readonly IValidator<UpdateUserFullNameCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // private readonly ICacheService _cache;
    public UpdateUserFullNameHandler(
        ILogger<UpdateUserFullNameHandler> logger,
        IValidator<UpdateUserFullNameCommand> validator,
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
        UpdateUserFullNameCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var userResult = await _userRepository.GetById(command.UserId, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        var fullname = FullName.Create(
            command.FullName.FirstName,
            command.FullName.SecondName,
            command.FullName.ThirdName).Value;

        userResult.Value.UpdateFullName(fullname);

        // var key = "users_" + userResult.Value.Id;
        //
        // var userCache = await _cache.GetAsync<UserDataModel>(key, cancellationToken);
        // if (userCache is not null)
        // {
        //     await _cache.RemoveAsync(key, cancellationToken);
        // }
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user full name successfully for {UserId}.", command.UserId);

        return userResult.Value.Id;
    }
}