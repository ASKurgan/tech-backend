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

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForIssue;

public class UpdateIssueAchievementConditionHandler : ICommandHandler<Guid, UpdateIssueAchievementConditionCommand>
{
    private readonly IAchievementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateIssueAchievementConditionCommand> _validator;
    private readonly ILogger<UpdateIssueAchievementConditionHandler> _logger;

    public UpdateIssueAchievementConditionHandler(
        IAchievementRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateIssueAchievementConditionCommand> validator,
        ILogger<UpdateIssueAchievementConditionHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateIssueAchievementConditionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var achievement = await _repository.GetById(command.Id, cancellationToken);
        if (achievement.IsFailure)
            return achievement.Error.ToErrorList();
        
        var difficulty = Difficulty.Create(command.IssueCondition.Difficulty).Value;

        var issueCondition = IssueCondition.Create(
            command.IssueCondition.TimeToComplete,
            difficulty,
            command.IssueCondition.Attempts,
            command.IssueCondition.IssueCount).Value;

        achievement.Value.UpdateCondition(issueCondition);;

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Created achievement {AchievementId}", achievement.Value.Id);

        return achievement.Value.Id.Value;
    }
}