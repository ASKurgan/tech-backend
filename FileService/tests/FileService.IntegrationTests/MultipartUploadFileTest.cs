using System.Net.Http.Json;
using FileService.Contracts;
using FluentAssertions;

namespace FileService.IntegrationTests;

public class MultipartUploadTests : FileServiceTestsBase
{
    public MultipartUploadTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_Success()
    {
        // Arrange
        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test.mp4"));
        var cancellationToken = new CancellationTokenSource().Token;

        // 1. Start Multipart Upload
        var startResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        var parts = new List<PartETagDto>();
        int partNumber = 1;

        await using var stream = fileInfo.OpenRead();

        // 2. Upload File Parts
        for (int i = 0; i < startResponse.TotalChunksCount; i++)
        {
            byte[] chunk = new byte[startResponse.ChunkSize];
            int bytesRead = await stream.ReadAsync(chunk, 0, (int)startResponse.ChunkSize, cancellationToken);

            if (bytesRead == 0)
                break;

            var chunkUrlResponse = await GenerateChunkUploadUrl(startResponse, partNumber, cancellationToken);

            var eTag = await UploadFilePartToMinio(
                chunkUrlResponse.UploadUrl,
                chunk,
                cancellationToken);

            parts.Add(new PartETagDto(partNumber, eTag!));
            partNumber++;
        }

        // 3. Complete Multipart Upload
        var completeResponse = await CompleteMultipartUpload(
            startResponse,
            parts,
            cancellationToken);

        completeResponse.Should().NotBeNull();
        completeResponse.FileId.Should().Be(startResponse.FileId);

        // 4. Get Download URL
        var downloadUrl = await GetDownloadUrl(
            startResponse,
            cancellationToken);

        downloadUrl.Should().NotBeNullOrEmpty();

        // 5. Validate the file exists in MinIO
        var httpResponse = await HttpClient.GetAsync(downloadUrl, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
    }

    private async Task<StartMultipartUploadResponse> StartMultipartUpload(
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        var request = new StartMultipartUploadRequest(
            fileInfo.Name,
            "videos",
            fileInfo.Length);

        var response = await AppHttpClient
            .PostAsJsonAsync("files/multipart/start", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken))!;
    }

    private async Task<GetChunkUploadUrlResponse> GenerateChunkUploadUrl(
        StartMultipartUploadResponse startResponse,
        int partNumber,
        CancellationToken cancellationToken)
    {
        var request = new GetChunkUploadUrlRequest(
            startResponse.FileId,
            startResponse.UploadId,
            partNumber,
            startResponse.BucketName);

        var response = await AppHttpClient
            .PostAsJsonAsync("files/multipart/chunk-url", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<GetChunkUploadUrlResponse>(cancellationToken))!;
    }

    private async Task<string> UploadFilePartToMinio(
        string uploadUrl,
        byte[] chunk,
        CancellationToken cancellationToken)
    {
        using var content = new ByteArrayContent(chunk);

        var response = await HttpClient.PutAsync(uploadUrl, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        return response.Headers.ETag?.Tag?.Trim('"')!;
    }

    private async Task<CompleteMultipartUploadResponse> CompleteMultipartUpload(
        StartMultipartUploadResponse startResponse,
        List<PartETagDto> parts,
        CancellationToken cancellationToken)
    {
        var request = new CompleteMultipartUploadRequest(
            startResponse.FileId,
            startResponse.UploadId,
            parts,
            startResponse.BucketName);

        var response = await AppHttpClient
            .PostAsJsonAsync("files/multipart/complete", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken))!;
    }

    private async Task<string> GetDownloadUrl(
        StartMultipartUploadResponse startResponse,
        CancellationToken cancellationToken)
    {
        var request = new GetDownloadUrlRequest(startResponse.FileId, startResponse.BucketName);

        var response = await AppHttpClient
            .PostAsJsonAsync("files/download-url", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var downloadResponse = await response.Content.ReadFromJsonAsync<GetDownloadUrlResponse>(cancellationToken);

        return downloadResponse?.DownloadUrl ?? string.Empty;
    }
}