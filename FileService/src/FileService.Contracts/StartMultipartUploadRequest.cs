namespace FileService.Contracts;

public record StartMultipartUploadRequest(
    string FileName,
    string BucketName,
    string ContentType,
    long FileSize);