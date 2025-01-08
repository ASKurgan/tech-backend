using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;
using UserProgressService.Application.Interfaces;

namespace UserProgressService.Application.Features.Achievements.Commands.Delete;

public class DeleteAchievementHandler : ICommandHandler<Guid, DeleteAchievementCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteAchievementCommand> _validator;
    private readonly ILogger<DeleteAchievementHandler> _logger;

    public DeleteAchievementHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteAchievementCommand> validator,
        ILogger<DeleteAchievementHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteAchievementCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievement = await _repository.GetById(command.Id, cancellationToken);
        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();

        var result = _repository.Delete(achievement.Value);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", result);

        return result;
    }
}