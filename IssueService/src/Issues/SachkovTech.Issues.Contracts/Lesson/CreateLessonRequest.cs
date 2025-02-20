using FileService.Contracts;

namespace SachkovTech.Issues.Contracts.Lesson;

public record CreateLessonRequest(
    Guid ModuleId,
    string Title,
    string Description,
    int Experience,
    IEnumerable<Guid> Tags,
    IEnumerable<Guid> Issues,
    CompleteMultipartUploadRequest MultipartRequest);