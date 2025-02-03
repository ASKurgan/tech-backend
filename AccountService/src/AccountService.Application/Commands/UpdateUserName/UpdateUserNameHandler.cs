using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Application.Database;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace ProjectTemplate.Application.Commands.UpdateUserName;

public class UpdateUserNameHandler : ICommandHandler<Guid, UpdateUserNameCommand>
{
    private readonly ILogger<UpdateUserNameHandler> _logger;
    private readonly IValidator<UpdateUserNameCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserNameHandler(
        ILogger<UpdateUserNameHandler> logger,
        IValidator<UpdateUserNameCommand> validator,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateUserNameCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var userResult = await _userRepository.GetById(command.Id, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        userResult.Value.UpdateUserName(command.UserName);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user full name successfully for {UserId}.", command.Id);

        return userResult.Value.Id;
    }
}