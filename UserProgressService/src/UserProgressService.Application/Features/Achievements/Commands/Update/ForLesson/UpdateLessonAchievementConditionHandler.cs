using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;
using UserProgressService.Application.Interfaces;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForLesson;

public class UpdateLessonAchievementConditionHandler : ICommandHandler<Guid, UpdateLessonAchievementConditionCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateLessonAchievementConditionCommand> _validator;
    private readonly ILogger<UpdateLessonAchievementConditionHandler> _logger;

    public UpdateLessonAchievementConditionHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateLessonAchievementConditionCommand> validator,
        ILogger<UpdateLessonAchievementConditionHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateLessonAchievementConditionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievement = await _repository.GetById(command.Id, cancellationToken);
        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();

        var difficulty = Difficulty.Create(command.LessonCondition.Difficulty).Value;

        var issueCondition = LessonCondition.Create(
            command.LessonCondition.TimeToComplete,
            difficulty,
            command.LessonCondition.RequiredCount).Value;

        achievement.Value.UpdateCondition(issueCondition);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", achievement.Value.Id);

        return achievement.Value.Id.Value;
    }
}