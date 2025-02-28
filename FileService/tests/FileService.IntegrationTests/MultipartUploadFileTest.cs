using System.Net.Http.Json;
using FileService.Contracts;
using FluentAssertions;
using SharedKernel;
using FileInfo = System.IO.FileInfo;

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
            "video/mp4",
            fileInfo.Length);

        var response = await AppHttpClient
            .PostAsJsonAsync("api/files/multipart/start", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Envelope<StartMultipartUploadResponse>>(cancellationToken);

        return result!.Result!;
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
            .PostAsJsonAsync("api/files/multipart/url", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Envelope<GetChunkUploadUrlResponse>>(cancellationToken);

        return result!.Result!;
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
            .PostAsJsonAsync("api/files/multipart/end", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Envelope<CompleteMultipartUploadResponse>>(cancellationToken);

        return result!.Result!;
    }

    private async Task<string> GetDownloadUrl(
        StartMultipartUploadResponse startResponse,
        CancellationToken cancellationToken)
    {
        var request = new GetDownloadUrlRequest(startResponse.FileId, startResponse.BucketName);

        var response = await AppHttpClient
            .PostAsJsonAsync("api/files/url", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Envelope<GetDownloadUrlResponse>>(cancellationToken);

        return result!.Result?.DownloadUrl ?? string.Empty;
    }
}