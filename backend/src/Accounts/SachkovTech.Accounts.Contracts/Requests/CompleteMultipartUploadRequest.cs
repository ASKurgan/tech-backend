using FileService.Contracts;
using SachkovTech.Accounts.Application.Requests;

namespace SachkovTech.Accounts.Contracts.Requests;

public record CompleteMultipartUploadRequest(
    FileMetadataRequest FileMetadata,
    string UploadId,
    List<PartETagInfo> Parts);