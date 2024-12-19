using CSharpFunctionalExtensions;

namespace UserProgressService.Domain.ValueObjects;

public class Level : ComparableValueObject
{
    private const int ExperiencePerLevel = 100;

    public int CurrentLevel { get; }
    public int CurrentExperience { get; }

    public Level(int currentExperience)
    {
        CurrentExperience = currentExperience;
        CurrentLevel = CalculateLevel(currentExperience);
    }

    private int CalculateLevel(int experience)
    {
        return experience / ExperiencePerLevel;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return CurrentLevel;
        yield return CurrentExperience;
    }
}