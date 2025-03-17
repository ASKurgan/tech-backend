using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain.Module.Entities;

public class LessonPosition : Entity<LessonPositionId>, IPositionable
{
    // ef core
    private LessonPosition()
    {
    }

    public LessonPosition(LessonPositionId id, LessonId lessonId, Position position)
        : base(id)
    {
        LessonId = lessonId;
        Position = position;
    }

    public LessonId LessonId { get; private set; }

    public Position Position { get; private set; }

    public void SetPosition(Position position)
    {
        Position = position;
    }
}