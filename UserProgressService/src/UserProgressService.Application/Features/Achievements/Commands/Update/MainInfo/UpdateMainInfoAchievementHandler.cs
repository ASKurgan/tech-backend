using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;
using UserProgressService.Application.Interfaces;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.MainInfo;

public class UpdateMainInfoAchievementHandler : ICommandHandler<Guid, UpdateMainInfoAchievementCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMainInfoAchievementCommand> _validator;
    private readonly ILogger<UpdateMainInfoAchievementHandler> _logger;

    public UpdateMainInfoAchievementHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMainInfoAchievementCommand> validator,
        ILogger<UpdateMainInfoAchievementHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateMainInfoAchievementCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievement = await _repository.GetById(command.Id, cancellationToken);
        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();

        achievement.Value.UpdateMainInfo(command.IconId, command.Name, command.Description, command.Experience);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", achievement.Value.Id);

        return achievement.Value.Id.Value;
    }
}