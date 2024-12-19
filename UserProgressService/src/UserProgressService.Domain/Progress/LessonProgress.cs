using CSharpFunctionalExtensions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Progress;

public class LessonProgress : Entity<LessonProgressId>
{
    public LessonProgress(Guid moduleId, Guid? lessonId, bool isCompleted)
    {
        ModuleId = moduleId;
        LessonId = lessonId;
        IsCompleted = isCompleted;
    }

    public Guid ModuleId { get; private set; }
    public Guid? LessonId { get; private set; }
    public bool IsCompleted { get; private set; }
}