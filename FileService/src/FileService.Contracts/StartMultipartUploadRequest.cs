namespace FileService.Contracts;

public record StartMultipartUploadRequest(
    string FileName,
    string BucketName,
    long FileSize);