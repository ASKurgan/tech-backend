namespace FileService.Contracts;

public record GenerateChunkUploadUrlRequest(
    string FileId,
    string UploadId,
    int PartNumber,
    string BucketName);