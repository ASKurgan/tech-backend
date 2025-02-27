using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Application.Mappers;

public static class LessonMapper
{
    public static LessonDto ToDto(
        this Lesson lesson,
        Dictionary<LessonId, Position>? positions,
        Dictionary<Video, string>? videoUrls) =>
        new()
        {
            Id = lesson.Id.Value,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Experience = lesson.Experience.Value,
            VideoUrl = videoUrls?[lesson.Video] ?? string.Empty,
            Position = positions?[lesson.Id] ?? 0,

            // TODO
            PreviewUrl = string.Empty,
            Tags = [],
            Issues = [],
        };
}