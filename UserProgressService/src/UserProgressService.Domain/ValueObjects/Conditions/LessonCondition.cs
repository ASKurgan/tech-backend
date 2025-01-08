using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using SharedKernel;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.ValueObjects.Conditions;

public sealed class LessonCondition : Condition
{
    [JsonConstructor]
    private LessonCondition(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int requiredCount)
        : base(timeToComplete, difficulty)
    {
        RequiredCount = requiredCount;
    }

    public int RequiredCount { get; }

    public static Result<LessonCondition, Error> Create(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int requiredCount)
    {
        if (timeToComplete.HasValue && timeToComplete.Value <= TimeSpan.Zero)
            return Errors.General.ValueIsInvalid("TimeToComplete");

        if (requiredCount <= 0)
            return Errors.General.ValueIsInvalid("RequiredCount");

        return new LessonCondition(timeToComplete, difficulty, requiredCount);
    }

    public override bool IsSatisfiedBy(UserProgress progress)
    {
        return progress.GetCompletedLessonCount() >= RequiredCount;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        foreach (var component in base.GetComparableEqualityComponents())
            yield return component;

        yield return RequiredCount;
    }
}