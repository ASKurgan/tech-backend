using MassTransit;

namespace FileService.Consumers;

public class VideoProcessConsumer : IConsumer<VideoUploadedEvent>
{
    private readonly VideoProcessor _videoProcessor;
    private readonly ILogger<VideoProcessConsumer> _logger;

    public VideoProcessConsumer(VideoProcessor videoProcessor, ILogger<VideoProcessConsumer> logger)
    {
        _videoProcessor = videoProcessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VideoUploadedEvent> context)
    {
        _logger.LogInformation("Video uploaded: {FileId}", context.Message.FileId);

        await _videoProcessor.ProcessVideoAsync(context.Message.BucketName, context.Message.FileId, context.CancellationToken);
    }
}