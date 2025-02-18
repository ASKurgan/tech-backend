namespace FileService.Contracts;

public record StartMultipartUploadResponse(
    string FileId,
    string UploadId,
    string BucketName,
    long ChunkSize,
    int TotalChunksCount);