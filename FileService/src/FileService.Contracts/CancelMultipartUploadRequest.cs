namespace FileService.Contracts;

public record CancelMultipartUploadRequest(FileLocation FileLocation, string UploadId);