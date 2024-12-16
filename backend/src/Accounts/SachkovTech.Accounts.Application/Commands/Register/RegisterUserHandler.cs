using CSharpFunctionalExtensions;
using FluentValidation;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Accounts.Application.Managers;
using SachkovTech.Accounts.Application.MessageBus;
using SachkovTech.Accounts.Contracts.Messaging;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountsManager _accountsManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IAccountsManager accountsManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger,
        Bind<IAccountMessageBus, IPublishEndpoint> publishEndpoint,
        IValidator<RegisterUserCommand> validator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountsManager = accountsManager;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _publishEndpoint = publishEndpoint.Value;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == ParticipantAccount.PARTICIPANT, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();

            var userResult = User.CreateParticipant(command.UserName, command.Email, role);

            if (userResult.IsFailure)
                return userResult.Error.ToErrorList();

            var user = userResult.Value;

            var result = await _userManager.CreateAsync(user, command.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Failure(e.Code, e.Description));
                return new ErrorList(errors);
            }

            var participantAccount = new ParticipantAccount(user);

            await _accountsManager.CreateParticipantAccount(participantAccount, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _publishEndpoint.Publish(new UserRegisteredEvent(user.Id), cancellationToken);

            _logger.LogInformation("User was created with name {userName}", command.UserName);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User registration was failed");

            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("register.user", "User registration was failed").ToErrorList();
        }
    }
}