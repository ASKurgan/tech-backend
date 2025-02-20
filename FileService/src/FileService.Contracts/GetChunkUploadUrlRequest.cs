namespace FileService.Contracts;

public record GetChunkUploadUrlRequest(
    string FileId,
    string UploadId,
    int PartNumber,
    string BucketName);