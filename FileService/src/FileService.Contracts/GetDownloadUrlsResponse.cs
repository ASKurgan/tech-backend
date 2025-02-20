namespace FileService.Contracts;

public record GetDownloadUrlsResponse(IEnumerable<FileUrl?> FileUrls);