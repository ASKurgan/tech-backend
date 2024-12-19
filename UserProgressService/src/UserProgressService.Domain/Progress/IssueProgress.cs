using CSharpFunctionalExtensions;
using UserProgressService.Domain.Enums;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Progress;

public class IssueProgress : Entity<IssueProgressId>
{
    public IssueProgress(
        IssueProgressId id,
        Guid moduleId,
        Guid issueId,
        int tryCount,
        DateTime startedDate,
        TimeSpan executionTime,
        Difficulty? difficulty) : base(id)
    {
        ModuleId = moduleId;
        IssueId = issueId;
        TryCount = tryCount;
        StartedDate = startedDate;
        ExecutionTime = executionTime;
        Difficulty = difficulty ?? Difficulty.None;
    }
    
    public Guid? ModuleId { get; private set; }
    public Guid IssueId { get; private set; }
    public int TryCount { get; private set; }
    public DateTime StartedDate { get; private set; }
    public TimeSpan ExecutionTime { get; private set; }
    public Difficulty Difficulty { get; private set; }    
}