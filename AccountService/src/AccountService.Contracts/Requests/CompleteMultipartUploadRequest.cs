using FileService.Contracts;

namespace AccountService.Contracts.Requests;

public record CompleteMultipartUploadRequest(
    FileMetadataRequest FileMetadata,
    string UploadId,
    List<PartETagInfo> Parts);