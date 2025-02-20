using FileService.Contracts;
using FileService.Extensions;
using FileService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Features;

public static class GetChunkUploadUrl
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/files/multipart/url", Handler);
        }
    }

    private static async Task<IResult> Handler(
        GetChunkUploadUrlRequest request,
        IS3Provider s3Provider,
        CancellationToken cancellationToken)
    {
        if (request.PartNumber <= 0)
        {
            return Results.BadRequest("PartNumber должен быть положительным числом.");
        }

        string uploadUrl = await s3Provider.GenerateChunkUploadUrl(
            new FileLocation(request.FileId, request.BucketName),
            request.UploadId,
            request.PartNumber);

        return Results.Ok(new GetChunkUploadUrlResponse(
            uploadUrl,
            request.PartNumber));
    }
}