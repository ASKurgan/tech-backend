using CSharpFunctionalExtensions;
using SharedKernel;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Progress;

public class LessonProgress : Entity<LessonProgressId>
{
    // EF Core
    private LessonProgress(LessonProgressId id) : base(id) { }

    public LessonProgress(
        LessonProgressId id,
        Guid moduleId,
        Guid? lessonId,
        bool isCompleted) : base(id)
    {
        ModuleId = moduleId;
        LessonId = lessonId;
        IsCompleted = isCompleted;
    }

    public Guid ModuleId { get; private set; }
    public Guid? LessonId { get; private set; }
    public bool IsCompleted { get; private set; }

    public static Result<LessonProgress, Error> Create(
        LessonProgressId id,
        Guid moduleId,
        Guid? lessonId,
        bool isCompleted)
    {
        if (moduleId == Guid.Empty)
            return Errors.General.ValueIsInvalid("ModuleId");

        if (lessonId.HasValue && lessonId.Value == Guid.Empty)
            return Errors.General.ValueIsInvalid("LessonId");

        return new LessonProgress(id, moduleId, lessonId, isCompleted);
    }
}