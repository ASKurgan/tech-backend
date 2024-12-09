namespace FileService.Providers;

public interface IFileProvider
{
    Task IsBucketExists(IEnumerable<string> bucketNames, CancellationToken cancellationToken = default);
}