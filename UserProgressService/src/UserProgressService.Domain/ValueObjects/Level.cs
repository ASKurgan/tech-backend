using CSharpFunctionalExtensions;
using SharedKernel;

namespace UserProgressService.Domain.ValueObjects;

public sealed class Level : ComparableValueObject
{
    private const int ExperiencePerLevel = 100;

    private Level(int currentExperience)
    {
        CurrentExperience = currentExperience;
        CurrentLevel = CalculateLevel(currentExperience);
    }

    public int CurrentLevel { get; }

    public int CurrentExperience { get; }

    public static Result<Level, Error> Create(int experience)
    {
        if (experience < 0)
            return Errors.General.ValueIsInvalid("Experience");

        return new Level(experience);
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