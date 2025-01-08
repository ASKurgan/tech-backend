using CSharpFunctionalExtensions;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.ValueObjects.Conditions;

public abstract class Condition : ComparableValueObject
{
    protected Condition(TimeSpan? timeToComplete, Difficulty difficulty)
    {
        TimeToComplete = timeToComplete;
        Difficulty = difficulty;
    }

    public TimeSpan? TimeToComplete { get; }

    public Difficulty Difficulty { get; }

    public abstract bool IsSatisfiedBy(UserProgress progress);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        if (TimeToComplete != null)
            yield return TimeToComplete;

        yield return Difficulty;
    }
}