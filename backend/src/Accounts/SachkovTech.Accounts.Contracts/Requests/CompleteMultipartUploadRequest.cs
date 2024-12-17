using FileService.Contracts;

namespace SachkovTech.Accounts.Contracts.Requests;

public record CompleteMultipartUploadRequest(
    FileMetadataRequest FileMetadata,
    string UploadId,
    List<PartETagInfo> Parts);