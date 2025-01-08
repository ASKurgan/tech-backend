using CSharpFunctionalExtensions;
using SharedKernel;
using UserProgressService.Domain.ValueObjects.Conditions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Achievements;

public sealed class Achievement : Entity<AchievementId>
{
    // Ef Core
    private Achievement(AchievementId id) : base(id) { }

    public Achievement(
        AchievementId id,
        Guid iconId,
        string name,
        string description,
        Condition condition,
        int experience) : base(id)
    {
        IconId = iconId;
        Name = name;
        Description = description;
        CreatedDate = DateOnly.FromDateTime(DateTime.Now);
        Condition = condition;
        Experience = experience;
    }

    public Guid IconId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateOnly CreatedDate { get; private set; }
    public int Experience { get; private set; }
    public Condition Condition { get; private set; }

    public void UpdateMainInfo(
        Guid iconId,
        string name,
        string description,
        int experience)
    {
        IconId = iconId;
        Name = name;
        Description = description;
        Experience = experience;
    }

    public void UpdateCondition(Condition condition) => Condition = condition;

    public static Result<Achievement, Error> Create(
        AchievementId id,
        Guid iconId,
        string name,
        string description,
        Condition condition,
        int experience)
    {
        if (iconId == Guid.Empty)
            return Errors.General.ValueIsInvalid("IconId");

        if (string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsInvalid("Name");

        if (string.IsNullOrWhiteSpace(description))
            return Errors.General.ValueIsInvalid("Description");

        if (experience <= 0)
            return Errors.General.ValueIsInvalid("Experience");

        return new Achievement(id, iconId, name, description, condition, experience);
    }
}