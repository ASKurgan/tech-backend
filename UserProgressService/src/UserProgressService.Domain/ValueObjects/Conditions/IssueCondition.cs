using UserProgressService.Domain.Enums;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.ValueObjects.Conditions;

public class IssueCondition : Condition
{
    public IssueCondition(
        TimeSpan? timeToComplete,
        Difficulty difficulty,
        int attempts,
        int issueCount)
        : base(timeToComplete, difficulty)
    {
        Attempts = attempts;
        IssueCount = issueCount;
    }

    public int? IssueCount { get; }
    public int? Attempts { get; }

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