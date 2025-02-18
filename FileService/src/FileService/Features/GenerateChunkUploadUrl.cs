using FileService.Contracts;
using FileService.Extensions;
using FileService.Services;

namespace FileService.Features;

public static class GenerateChunkUploadUrl
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart/chunk-url", Handler);
        }
    }

    private static async Task<IResult> Handler(
        GenerateChunkUploadUrlRequest request,
        IS3Provider s3Provider,
        CancellationToken cancellationToken)
    {
        if (request.PartNumber <= 0)
        {
            return Results.BadRequest("PartNumber должен быть положительным числом.");
        }

        // Генерация ссылки для загрузки чанка
        string uploadUrl = await s3Provider.GenerateChunkUploadUrl(
            request.BucketName,
            request.FileId,
            request.UploadId,
            request.PartNumber);

        return Results.Ok(new GenerateChunkUploadUrlResponse(
            uploadUrl,
            request.PartNumber));
    }
}