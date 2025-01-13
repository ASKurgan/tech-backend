using FileService.Contracts;
using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;

public record AddLessonCommand(
    Guid ModuleId,
    string Title,
    string Description,
    int Experience,
    Guid VideoId,
    Guid PreviewId,
    IEnumerable<Guid> Tags,
    IEnumerable<Guid> Issues,
    string FileName,
    string ContentType,
    long FileSize,
    string UploadId,
    List<PartETagInfo> Parts) : ICommand;
