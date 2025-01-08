using CSharpFunctionalExtensions;
using SharedKernel;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Progress;

public class IssueProgress : Entity<IssueProgressId>
{
    // EF Core
    private IssueProgress(IssueProgressId id) : base(id) { }

    public IssueProgress(
        IssueProgressId id,
        Guid moduleId,
        Guid issueId,
        int tryCount,
        TimeSpan executionTime,
        Difficulty difficulty) : base(id)
    {
        ModuleId = moduleId;
        IssueId = issueId;
        TryCount = tryCount;
        StartedDate = DateOnly.FromDateTime(DateTime.Now);
        ExecutionTime = executionTime;
        Difficulty = difficulty;
    }

    public Guid? ModuleId { get; private set; }
    public Guid IssueId { get; private set; }
    public int TryCount { get; private set; }
    public DateOnly StartedDate { get; private set; }
    public TimeSpan ExecutionTime { get; private set; }
    public Difficulty Difficulty { get; private set; }

    public static Result<IssueProgress, Error> Create(
        IssueProgressId id,
        Guid moduleId,
        Guid issueId,
        int tryCount,
        TimeSpan executionTime,
        Difficulty difficulty)
    {
        if (moduleId == Guid.Empty)
            return Errors.General.ValueIsInvalid("ModuleId");

        if (issueId == Guid.Empty)
            return Errors.General.ValueIsInvalid("IssueId");

        if (tryCount < 0)
            return Errors.General.ValueIsInvalid("TryCount");
        
        if (executionTime <= TimeSpan.Zero)
            return Errors.General.ValueIsInvalid("ExecutionTime");

        return new IssueProgress(id, moduleId, issueId, tryCount, executionTime, difficulty);
    }
}