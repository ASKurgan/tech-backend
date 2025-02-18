namespace FileService.Consumers;

public record VideoUploadedEvent(string BucketName, string FileId);