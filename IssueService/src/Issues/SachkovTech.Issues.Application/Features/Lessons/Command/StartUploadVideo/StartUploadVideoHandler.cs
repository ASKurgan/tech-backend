using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.StartUploadVideo;

public class StartUploadVideoHandler : ICommandHandler<StartUploadFileResponse, StartUploadVideoCommand>
{
    private readonly IFileService _fileService;

    public StartUploadVideoHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<Result<StartUploadFileResponse, ErrorList>> Handle(StartUploadVideoCommand command, CancellationToken cancellationToken = default)
    {
        var validateResult = Video.Validate(
            command.FileName,
            command.ContentType,
            command.FileSize);

        if (validateResult.IsFailure)
            return validateResult.Error.ToErrorList();

        var startMultipartRequest = new StartMultipartUploadRequest(
            command.FileName,
            command.ContentType,
            command.FileSize);

        var result = await _fileService.StartMultipartUpload(
            startMultipartRequest,
            cancellationToken);

        if (result.IsFailure)
            return Errors.General.ValueIsInvalid(result.Error).ToErrorList();

        return new StartUploadFileResponse(result.Value.FileId, result.Value.PresignedUrl);
    }
}
