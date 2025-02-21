using FileService.Contracts;
using FileService.Services;
using MassTransit;
using SachkovTech.Framework.Authorization;
using SachkovTech.Framework.Endpoints;
using SharedKernel;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/files/multipart/end", Handler)
                .RequireAuthorization(Permissions.Files.UPLOAD_FILES);
        }
    }

    private static async Task<IResult> Handler(
        CompleteMultipartUploadRequest request,
        IS3Provider s3Provider,
        // VideoProcessor videoProcessor,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (request.PartETags.Count == 0)
        {
            return ResultResponse.BadRequest(Errors.General.ValueIsInvalid("PartETags должен содержать хотя бы одну часть."));
        }

        var partETags = request.PartETags
            .Select(p => (p.PartNumber, p.ETag))
            .ToList();

        string key = await s3Provider.CompleteMultipartUploadAsync(
            new FileLocation(request.FileId, request.BucketName),
            request.UploadId,
            partETags,
            cancellationToken);

        // await publishEndpoint.Publish(new VideoUploadedEvent("videos", key), cancellationToken);
        return ResultResponse.Ok(new CompleteMultipartUploadResponse(key));
    }
}