using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SharedKernel;

namespace SachkovTech.Issues.IntegrationTests.Lessons;

public class LessonTestWebFactory : IntegrationTestsWebFactory
{
    private readonly IFileService _fileServiceMock = Substitute.For<IFileService>();

    public void SetupSuccessFileServiceMock()
    {
        var response = new CompleteMultipartUploadResponse("testUrl");
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<CompleteMultipartUploadResponse, ErrorList>(response));

        _fileServiceMock
            .GetDownloadUrls(Arg.Any<GetDownloadUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<GetDownloadUrlsResponse, ErrorList>(new GetDownloadUrlsResponse([new FileUrl("test", "test")])));
    }

    public void SetupSuccessFileServiceMock(IEnumerable<Guid> fileIds)
    {
        var response = new CompleteMultipartUploadResponse("testUrl");
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<CompleteMultipartUploadResponse, ErrorList>(response));

        var urls = fileIds
            .Select(id => new FileUrl($"testUrl/{id}", $"test/{id}"))
            .ToArray();
        _fileServiceMock
            .GetDownloadUrls(Arg.Any<GetDownloadUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<GetDownloadUrlsResponse, ErrorList>(new GetDownloadUrlsResponse(urls)));
    }

    public void SetupFailureFileServiceMock()
    {
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<CompleteMultipartUploadResponse, ErrorList>(Errors.General.Failure()));

        _fileServiceMock
            .GetDownloadUrls(Arg.Any<GetDownloadUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<GetDownloadUrlsResponse, ErrorList>(Errors.General.Failure()));
    }

    protected override void ConfigureDefaultServices(IServiceCollection services)
    {
        base.ConfigureDefaultServices(services);

        var fileServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IFileService));
        if (fileServiceDescriptor != null)
            services.Remove(fileServiceDescriptor);

        services.AddTransient<IFileService>(_ => _fileServiceMock);
    }
}