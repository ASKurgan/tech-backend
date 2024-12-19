using UserProgressService.Domain.Enums;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.ValueObjects.Conditions;

public class LessonCondition : Condition
{
    private readonly int _requiredCount;

    public LessonCondition(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int requiredCount)
        : base(timeToComplete, difficulty)
    {
        _requiredCount = requiredCount;
    }

    public override bool IsSatisfiedBy(UserProgress progress)
    {
        return progress.GetCompletedLessonCount() >= _requiredCount;
    }
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        foreach (var component in base.GetComparableEqualityComponents())
            yield return component;
            
        yield return _requiredCount;
    }
}