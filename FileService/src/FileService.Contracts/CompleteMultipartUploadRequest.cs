namespace FileService.Contracts;

public record CompleteMultipartUploadRequest(
    string FileId,
    string UploadId,
    List<PartETagDto> PartETags,
    string BucketName);