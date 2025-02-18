using AccountService.Application.Database;
using AccountService.Domain;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;

namespace AccountService.Application.Commands.UpdateProfile;

public class UpdateProfileHandler : ICommandHandler<Guid, UpdateProfileCommand>
{
    private readonly ILogger<UpdateProfileHandler> _logger;
    private readonly IValidator<UpdateProfileCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(
        ILogger<UpdateProfileHandler> logger,
        IValidator<UpdateProfileCommand> validator,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        bool isUserExist = await _userRepository.IsUserExistsByUserName(command.Dto.UserName, cancellationToken);
        if (isUserExist)
            return UserErrors.UserAlreadyExist();

        var userResult = await _userRepository.GetById(command.Id, cancellationToken);
        if (userResult.IsFailure)
            return userResult.Error.ToErrorList();

        var fullname = FullName.Create(command.Dto.FirstName, command.Dto.SecondName, command.Dto.ThirdName).Value;

        var socials = command.Dto.Socials
            .Select(s => SocialNetwork.Create(s.Name, s.Link).Value);

        var updateResult = userResult.Value.UpdateProfile(command.Dto.UserName, fullname, socials);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Updated user full name successfully for {UserId}.", command.Id);

        return userResult.Value.Id;
    }
}