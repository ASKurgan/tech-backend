namespace FileService.Contracts;

public record GetDownloadUrlRequest(
    string FileId,
    string BucketName);