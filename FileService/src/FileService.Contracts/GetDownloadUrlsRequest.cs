namespace FileService.Contracts;

public record GetDownloadUrlsRequest(IEnumerable<FileLocation> Locations);