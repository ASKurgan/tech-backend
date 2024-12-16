namespace SachkovTech.Issues.Application.Requests;

public record FileMetadataRequest(string FileName, string ContentType, long FileSize);
