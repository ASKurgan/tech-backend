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
    /// <summary>
    /// Инициализация multipart-загрузки большого файла.
    /// </summary>
    /// <param name="request">Содержит имя, тип и размер файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с идентификатором загрузки и URL для загрузки файла.</returns>
    public async Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("files/multipart", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to start multipart upload: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken);

        return result!;
    }

    /// <summary>
    /// Завершение multipart-загрузки большого файла.
    /// </summary>
    /// <param name="request">Содержит данные о частях и идентификатор загрузки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с идентификатором файла.</returns>
    public async Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartUploadRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("files/multipart/complete", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to complete multipart upload: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken);

        return result!;
    }

    /// <summary>
    /// Генерация предподписанной ссылки для загрузки чанка.
    /// </summary>
    /// <param name="request">Содержит данные о части файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с URL для загрузки чанка.</returns>
    public async Task<Result<GenerateChunkUploadUrlResponse, string>> GenerateChunkUploadUrl(
        GenerateChunkUploadUrlRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("files/multipart/chunk-url", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to generate chunk upload URL: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<GenerateChunkUploadUrlResponse>(cancellationToken);

        return result!;
    }

    /// <summary>
    /// Получение ссылки на скачивание файла.
    /// </summary>
    /// <param name="request">Содержит идентификатор файла и имя бакета.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с URL для скачивания файла.</returns>
    public async Task<Result<GetDownloadUrlResponse, string>> GetDownloadUrl(
        GetDownloadUrlRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("files/download-url", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return $"Failed to generate download URL: {response.ReasonPhrase}";
        }

        var result = await response.Content.ReadFromJsonAsync<GetDownloadUrlResponse>(cancellationToken);

        return result!;
    }
}