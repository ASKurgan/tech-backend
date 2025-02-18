using Amazon.S3;
using Amazon.S3.Model;

namespace FileService.Services;

public class S3Provider : IS3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly FileTypeResolver _fileTypeResolver;

    public S3Provider(IAmazonS3 s3Client, FileTypeResolver fileTypeResolver)
    {
        _s3Client = s3Client;
        _fileTypeResolver = fileTypeResolver;
    }

    /// <summary>
    /// Инициализация multipart-загрузки
    /// </summary>
    /// <param name="fileName">Название файла.</param>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ multipart-загрузки из S3 клиента.</returns>
    public async Task<string> StartMultipartUpload(
        string fileName,
        string bucketName,
        string fileId,
        CancellationToken cancellationToken)
    {
        await CreateBucketIfNotExists(bucketName, cancellationToken);

        var initiateRequest = new InitiateMultipartUploadRequest
        {
            BucketName = bucketName, Key = fileId,
        };

        initiateRequest.Metadata.Add("file-name", fileName);

        var result = await _s3Client.InitiateMultipartUploadAsync(initiateRequest, cancellationToken);

        return result.UploadId;
    }

    /// <summary>
    /// Генерация предподписанной ссылки для загрузки чанка
    /// </summary>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="uploadId">Идентификатор multipart-загрузки.</param>
    /// <param name="partNumber">Номер части.</param>
    /// <returns>Предподписанная ссылка для загрузки чанка.</returns>
    public async Task<string> GenerateChunkUploadUrl(
        string bucketName,
        string fileId,
        string uploadId,
        int partNumber)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = fileId,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(60),
            PartNumber = partNumber,
            UploadId = uploadId,
            Protocol = Protocol.HTTP,
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    /// <summary>
    /// Завершение multipart-загрузки
    /// </summary>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="uploadId">Идентификатор multipart-загрузки.</param>
    /// <param name="partETags">Список тегов чанков.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>S3 ключ.</returns>
    public async Task<string> CompleteMultipartUploadAsync(
        string bucketName,
        string fileId,
        string uploadId,
        List<(int PartNumber, string ETag)> partETags,
        CancellationToken cancellationToken)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = fileId,
            UploadId = uploadId,
            PartETags = partETags.Select(pt => new PartETag(pt.PartNumber, pt.ETag)).ToList(),
        };

        var response = await _s3Client.CompleteMultipartUploadAsync(completeRequest, cancellationToken);

        return response.Key;
    }

    /// <summary>
    /// Генерация ссылки на скачивание файла.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="expirationHours">Время жизни ссылки.</param>
    /// <returns>Ссылка на скачивание файла.</returns>
    public async Task<string> GenerateDownloadUrlAsync(string bucketName, string fileId, int expirationHours)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = fileId,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Protocol = Protocol.HTTP,
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    /// <summary>
    /// Загрузка файла из S3 в указанную дерикторию.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="tempInputPath">Временная директория для хранения файла.</param>
    /// <param name="cancellationToken">Токен отметы.</param>
    /// <returns>Task.</returns>
    public async Task DownloadFileAsync(string bucketName, string fileId, string tempInputPath, CancellationToken cancellationToken)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName, Key = fileId,
        };

        using var response = await _s3Client.GetObjectAsync(request, cancellationToken);
        await using var fileStream = File.Create(tempInputPath);
        await response.ResponseStream.CopyToAsync(fileStream, cancellationToken);
    }

    /// <summary>
    /// Загрузка файла в S3-хранилище.
    /// </summary>
    /// <param name="bucketName">Название бакета.</param>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="file">Поток файла для загрузки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача выполнения загрузки файла.</returns>
    public async Task UploadFileAsync(string bucketName, string fileId, Stream file, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName, Key = fileId, InputStream = file, // Укажите MIME-тип, если известен
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
    }

    public async Task<string> GenerateUploadUrl(string bucketName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = Guid.NewGuid().ToString(),
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Protocol = Protocol.HTTP,
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    private async Task CreateBucketIfNotExists(string bucketName, CancellationToken cancellationToken)
    {
        var response = await _s3Client.ListBucketsAsync(cancellationToken);
        if (response.Buckets.Any(b => b.BucketName.Equals(bucketName, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        // Создание бакета
        var bucketRequest = new PutBucketRequest
        {
            BucketName = bucketName, UseClientRegion = true,
        };

        await _s3Client.PutBucketAsync(bucketRequest, cancellationToken);
    }
}