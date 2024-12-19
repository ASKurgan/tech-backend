using CSharpFunctionalExtensions;
using UserProgressService.Domain.Enums;
using UserProgressService.Domain.ValueObjects.Conditions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Achievements;

public sealed class Achievement : Entity<AchievementId>
{
    public Achievement(
        AchievementId id,
        Guid iconId,
        string name,
        string description,
        DateOnly createdDate,
        Condition condition,
        int experience) : base(id)
    {
        IconId = iconId;
        Name = name;
        Description = description;
        CreatedDate = createdDate;
        Condition = condition;
        Experience = experience;
    }

    public Guid IconId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateOnly CreatedDate { get; private set; }
    public Condition Condition { get; private set; }
    public int Experience { get; private set; }
}