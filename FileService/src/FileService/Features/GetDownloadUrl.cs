using FileService.Contracts;
using FileService.Extensions;
using FileService.Services;

namespace FileService.Features;

public static class GetDownloadUrl
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/download-url", Handler);
        }
    }

    private static async Task<IResult> Handler(
        GetDownloadUrlRequest request,
        IS3Provider s3Provider,
        CancellationToken cancellationToken)
    {
        // Валидация входных данных
        if (string.IsNullOrEmpty(request.FileId))
        {
            return Results.BadRequest("FileId обязателен.");
        }

        // Генерация ссылки на скачивание
        string downloadUrl = await s3Provider.GenerateDownloadUrlAsync(
            request.BucketName,
            request.FileId,
            24);

        return Results.Ok(new GetDownloadUrlResponse(downloadUrl));
    }
}