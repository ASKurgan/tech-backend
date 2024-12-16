namespace SachkovTech.Accounts.Application.Requests;
public record FileMetadataRequest(string FileName, string ContentType, long FileSize);
