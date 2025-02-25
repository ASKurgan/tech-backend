using FileService.Contracts;
using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.CreateLesson;

public record CreateLessonCommand(
    Guid ModuleId,
    string Title,
    string Description,
    int Experience,
    IEnumerable<Guid> Tags,
    IEnumerable<Guid> Issues,
    CompleteMultipartUploadRequest MultipartRequest) : ICommand;