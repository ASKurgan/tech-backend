using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SharedKernel;
using UserProgressService.Application.Extentions;
using UserProgressService.Application.Interfaces;
using UserProgressService.Application.Responses;

namespace UserProgressService.Application.Features.Achievements.Queries.GetAchievementById;

public class GetAchievementByIdHandler : IQueryHandlerWithResult<AchievementResponse, GetAchievementByIdQuery>
{
    private readonly IDbContext _dbContext;

    public GetAchievementByIdHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<AchievementResponse, ErrorList>> Handle(
        GetAchievementByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var achievement = await _dbContext.AchievementsQuery
            .SingleOrDefaultAsync(a => a.Id == query.AchievementId, cancellationToken);
        if (achievement is null)
            return Errors.General.NotFound(query.AchievementId.Value).ToErrorList();

        var conditionDto = Mapper.MapConditionToDto(achievement.Condition);

        return new AchievementResponse
        {
            Id = achievement.Id,
            IconId = achievement.IconId,
            Name = achievement.Name,
            Description = achievement.Description,
            CreatedDate = achievement.CreatedDate,
            Experience = achievement.Experience,
            Condition = conditionDto,
        };
    }
}