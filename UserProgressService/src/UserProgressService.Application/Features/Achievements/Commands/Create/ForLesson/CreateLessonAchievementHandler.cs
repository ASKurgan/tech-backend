using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SharedKernel;
using UserProgressService.Application.Interfaces;
using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Commands.Create.ForLesson;

public class CreateLessonAchievementHandler : ICommandHandler<Guid, CreateLessonAchievementCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateLessonAchievementCommand> _validator;
    private readonly ILogger<CreateLessonAchievementHandler> _logger;

    public CreateLessonAchievementHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateLessonAchievementCommand> validator,
        ILogger<CreateLessonAchievementHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        CreateLessonAchievementCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievementId = AchievementId.NewId();

        var difficulty = Difficulty.Create(command.LessonCondition.Difficulty).Value;

        var lessonCondition = LessonCondition.Create(
            command.LessonCondition.TimeToComplete,
            difficulty,
            command.LessonCondition.RequiredCount).Value;

        var achievement = Achievement.Create(
            achievementId,
            command.IconId,
            command.Name,
            command.Description,
            lessonCondition,
            command.Experience);

        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();

        var result = await _repository.Add(achievement.Value, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", result);

        return result;
    }
}