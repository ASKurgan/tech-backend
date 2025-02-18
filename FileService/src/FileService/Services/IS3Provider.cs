namespace FileService.Services;

public interface IS3Provider
{
    /// <summary>
    /// Инициализация multipart-загрузки
    /// </summary>
    /// <param name="fileName">Название файла.</param>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ multipart-загрузки из S3 клиента.</returns>
    Task<string> StartMultipartUpload(
        string fileName,
        string bucketName,
        string fileId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Генерация предподписанной ссылки для загрузки чанка
    /// </summary>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="uploadId">Идентификатор multipart-загрузки.</param>
    /// <param name="partNumber">Номер части.</param>
    /// <returns>Предподписанная ссылка для загрузки чанка.</returns>
    Task<string> GenerateChunkUploadUrl(
        string bucketName,
        string fileId,
        string uploadId,
        int partNumber);

    /// <summary>
    /// Завершение multipart-загрузки
    /// </summary>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="uploadId">Идентификатор multipart-загрузки.</param>
    /// <param name="partETags">Список тегов чанков.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>S3 ключ.</returns>
    Task<string> CompleteMultipartUploadAsync(
        string bucketName,
        string fileId,
        string uploadId,
        List<(int PartNumber, string ETag)> partETags,
        CancellationToken cancellationToken);

    /// <summary>
    /// Генерация ссылки на скачивание файла.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="expirationHours">Время жизни ссылки.</param>
    /// <returns>Ссылка на скачивание файла.</returns>
    Task<string> GenerateDownloadUrlAsync(string bucketName, string fileId, int expirationHours);

    /// <summary>
    /// Загрузка файла из S3 в указанную дерикторию.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="tempInputPath">Временная директория для хранения файла.</param>
    /// <param name="cancellationToken">Токен отметы.</param>
    /// <returns>Task.</returns>
    Task DownloadFileAsync(string bucketName, string fileId, string tempInputPath, CancellationToken cancellationToken);

    /// <summary>
    /// Загрузка файла в бакет с помощью Stream.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="file">Поток файла.</param>
    /// <param name="cancellationToken">Токен отметы.</param>
    /// <returns>Task.</returns>
    Task UploadFileAsync(string bucketName, string fileId, Stream file, CancellationToken cancellationToken);
}