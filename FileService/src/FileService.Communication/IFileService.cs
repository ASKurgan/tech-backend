using CSharpFunctionalExtensions;
using FileService.Contracts;

namespace FileService.Communication;

public interface IFileService
{
    /// <summary>
    /// Инициализация multipart-загрузки большого файла.
    /// </summary>
    /// <param name="request">Содержит имя, тип и размер файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с идентификатором загрузки и URL для загрузки файла.</returns>
    Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Завершение multipart-загрузки большого файла.
    /// </summary>
    /// <param name="request">Содержит данные о частях и идентификатор загрузки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с идентификатором файла.</returns>
    Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartUploadRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Генерация предподписанной ссылки для загрузки чанка.
    /// </summary>
    /// <param name="request">Содержит данные о части файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с URL для загрузки чанка.</returns>
    Task<Result<GenerateChunkUploadUrlResponse, string>> GenerateChunkUploadUrl(
        GenerateChunkUploadUrlRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Получение ссылки на скачивание файла.
    /// </summary>
    /// <param name="request">Содержит идентификатор файла и имя бакета.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с URL для скачивания файла.</returns>
    Task<Result<GetDownloadUrlResponse, string>> GetDownloadUrl(
        GetDownloadUrlRequest request, CancellationToken cancellationToken);
}