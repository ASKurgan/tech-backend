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

namespace UserProgressService.Application.Features.Achievements.Commands.Create.ForIssue;

public class CreateIssueAchievementHandler : ICommandHandler<Guid, CreateIssueAchievementCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateIssueAchievementCommand> _validator;
    private readonly ILogger<CreateIssueAchievementHandler> _logger;

    public CreateIssueAchievementHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateIssueAchievementCommand> validator,
        ILogger<CreateIssueAchievementHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        CreateIssueAchievementCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievementId = AchievementId.NewId();

        var difficulty = Difficulty.Create(command.IssueCondition.Difficulty).Value;

        var issueCondition = IssueCondition.Create(
            command.IssueCondition.TimeToComplete,
            difficulty,
            command.IssueCondition.Attempts,
            command.IssueCondition.IssueCount).Value;

        var achievement = Achievement.Create(
            achievementId,
            command.IconId,
            command.Name,
            command.Description,
            issueCondition,
            command.Experience);

        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();

        var result = await _repository.Add(achievement.Value, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", result);

        return result;
    }
}