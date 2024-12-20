using CSharpFunctionalExtensions;
using FileService.Communication;

using FileService.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace SachkovTech.Issues.IntegrationTests.Lessons;

public class LessonTestWebFactory : IntegrationTestsWebFactory
{
    private readonly IFileService _fileServiceMock = Substitute.For<IFileService>();

    protected override void ConfigureDefaultServices(IServiceCollection services)
    {
        base.ConfigureDefaultServices(services);

        var fileServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IFileService));
        if (fileServiceDescriptor != null)
            services.Remove(fileServiceDescriptor);

        services.AddTransient<IFileService>(_ => _fileServiceMock);
    }

    public void SetupSuccessFileServiceMock()
    {
        var response = new FileResponse(Guid.NewGuid(), "testUrl");
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<FileResponse, string>(response));
        
        _fileServiceMock
            .GetFilesPresignedUrls(Arg.Any<GetFilesPresignedUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<FileResponse>, string>([response]));
        
    }
    public void SetupSuccessFileServiceMock(IEnumerable<Guid> fileIds)
    {
        var responses = fileIds.Select(id => new FileResponse(id, "testUrl")).ToList();
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<FileResponse, string>(responses.First()));
        
        _fileServiceMock
            .GetFilesPresignedUrls(Arg.Any<GetFilesPresignedUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<FileResponse>, string>(responses));
    }

    public void SetupFailureFileServiceMock()
    {
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<FileResponse, string>("Failed to upload file"));
        
        _fileServiceMock
            .GetFilesPresignedUrls(Arg.Any<GetFilesPresignedUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IReadOnlyList<FileResponse>, string>("Failed to upload file"));
    }
}