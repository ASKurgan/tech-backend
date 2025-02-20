using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts;

namespace FileService.Communication;

/// <summary>
/// Клиент для взаимодействия с файловыми эндпоинтами.
/// </summary>
public class FileHttpClient(HttpClient httpClient) : IFileService
{
    public async Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/files/multipart/start", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken);
        return result!;
    }

    public async Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartUploadRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/files/multipart/end", request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to complete multipart upload: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken);
        return result!;
    }

    public async Task<Result<GetChunkUploadUrlResponse, string>> GetChunkUploadUrl(
        GetChunkUploadUrlRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/files/multipart/url", request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to generate chunk upload URL: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<GetChunkUploadUrlResponse>(cancellationToken);
        return result!;
    }

    public async Task<Result<GetDownloadUrlResponse, string>> GetDownloadUrl(
        GetDownloadUrlRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/files/url", request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to generate download URL: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<GetDownloadUrlResponse>(cancellationToken);
        return result!;
    }

    public async Task<Result<GetDownloadUrlsResponse, string>> GetDownloadUrls(
        GetDownloadUrlsRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/files/urls", request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to generate download URLs: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<GetDownloadUrlsResponse>(cancellationToken);
        return result!;
    }
}