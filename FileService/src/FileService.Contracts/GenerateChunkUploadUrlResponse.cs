namespace FileService.Contracts;

public record GenerateChunkUploadUrlResponse(
    string UploadUrl,
    int PartNumber);