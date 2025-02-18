using FileService.Contracts;
using FileService.Extensions;
using FileService.Services;
using MassTransit;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart/complete", Handler);
        }
    }

    private static async Task<IResult> Handler(
        CompleteMultipartUploadRequest request,
        IS3Provider s3Provider,
        VideoProcessor videoProcessor,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (request.PartETags.Count == 0)
        {
            return Results.BadRequest("PartETags должен содержать хотя бы одну часть.");
        }

        var partETags = request.PartETags
            .Select(p => (p.PartNumber, p.ETag))
            .ToList();

        string key = await s3Provider.CompleteMultipartUploadAsync(
            request.BucketName,
            request.FileId,
            request.UploadId,
            partETags,
            cancellationToken);

        //await publishEndpoint.Publish(new VideoUploadedEvent("videos", key), cancellationToken);

        return Results.Ok(new CompleteMultipartUploadResponse(key));
    }
}