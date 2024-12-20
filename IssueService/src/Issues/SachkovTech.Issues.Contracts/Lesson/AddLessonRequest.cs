using FileService.Contracts;

namespace SachkovTech.Issues.Contracts.Lesson;

public record AddLessonRequest(
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
    List<PartETagInfo> Parts);