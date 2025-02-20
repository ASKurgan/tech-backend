using FileService.Contracts;
using FileService.Extensions;
using FileService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Features;

public static class GetDownloadUrls
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/files/urls", Handler);
        }
    }

    private static async Task<IResult> Handler(
        GetDownloadUrlsRequest request,
        IS3Provider s3Provider,
        CancellationToken cancellationToken)
    {
        if (!request.Locations.Any())
        {
            return Results.BadRequest("FileIds cannot be empty.");
        }

        var downloadUrls = await s3Provider.GenerateDownloadUrlsAsync(
            request.Locations,
            24);

        return Results.Ok(new GetDownloadUrlsResponse(downloadUrls));
    }
}