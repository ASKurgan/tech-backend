using FileService.Extensions;
using FileService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Features;

public static class GetHlsMasterPlaylist
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("videos/{fileId}/hls", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] string fileId,
        IS3Provider s3Provider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            return Results.BadRequest("FileId is required.");
        }

        try
        {
            // Получаем пресайгненный URL для .m3u8 файла
            var masterPlaylistUrl = await s3Provider.GenerateDownloadUrlAsync("videos", $"{fileId}/hls/master.m3u8", 24);

            // Проверяем существование файла
            if (string.IsNullOrEmpty(masterPlaylistUrl))
            {
                return Results.NotFound("HLS master playlist not found.");
            }

            return Results.Ok(new
            {
                url = masterPlaylistUrl,
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving HLS master playlist: {ex.Message}");
        }
    }
}