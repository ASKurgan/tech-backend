using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Lesson;

namespace SachkovTech.Issues.Application.Mappers;

public static class LessonMapper
{
    public static LessonDto ToDto(this Lesson lesson, Dictionary<string, string>? videoUrls) =>
        new()
        {
            Id = lesson.Id.Value,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Experience = lesson.Experience.Value,
            VideoUrl = videoUrls?[lesson.Video.FileId.ToString()] ?? string.Empty,

            // TODO
            PreviewUrl = "",
            Tags = [],
            Issues = [],
        };
}