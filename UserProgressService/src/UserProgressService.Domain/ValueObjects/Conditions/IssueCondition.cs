

using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using SharedKernel;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.ValueObjects.Conditions;

public sealed class IssueCondition : Condition
{
    [JsonConstructor]
    private IssueCondition(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int? attempts,
        int? issueCount)
        : base(timeToComplete, difficulty)
    {
        Attempts = attempts;
        IssueCount = issueCount;
    }

    public int? Attempts { get; }

    public int? IssueCount { get; }

    public static Result<IssueCondition, Error> Create(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int? attempts,
        int? issueCount)
    {
        if (timeToComplete.HasValue && timeToComplete.Value <= TimeSpan.Zero)
            return Errors.General.ValueIsInvalid("TimeToComplete");

        if (issueCount.HasValue && issueCount.Value <= 0)
            return Errors.General.ValueIsInvalid("IssueCount");

        if (attempts.HasValue && attempts.Value <= 0)
            return Errors.General.ValueIsInvalid("Attempts");

        return new IssueCondition(timeToComplete, difficulty, attempts, issueCount);
    }

    public override bool IsSatisfiedBy(UserProgress progress)
    {
        var validIssues = progress.GetIssues(
            Difficulty,
            TimeToComplete,
            Attempts);

        if (!IssueCount.HasValue)
            return validIssues.Count > 0;

        if (!TimeToComplete.HasValue)
            return validIssues.Count >= IssueCount.Value;

        for (var i = 0; i <= validIssues.Count - IssueCount.Value; i++)
        {
            var subset = validIssues.Skip(i).Take(IssueCount.Value).ToList();
            var totalExecutionTime = subset.Sum(x => x.ExecutionTime.Ticks);

            if (new TimeSpan(totalExecutionTime) <= TimeToComplete.Value)
                return true;
        }

        return false;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        foreach (var component in base.GetComparableEqualityComponents())
            yield return component;

        if (IssueCount != null)
            yield return IssueCount;

        if (Attempts != null)
            yield return Attempts;
    }
}