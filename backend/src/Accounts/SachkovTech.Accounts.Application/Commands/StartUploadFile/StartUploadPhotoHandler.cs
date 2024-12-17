using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Accounts.Contracts.Responses;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Abstractions;
using SachkovTech.SharedKernel.ValueObjects;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.StartUploadFile;

public class StartUploadPhotoHandler : ICommandHandler<StartUploadFileResponse, StartUploadPhotoCommand>
{
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;

    public StartUploadPhotoHandler(IFileService fileService, UserManager<User> userManager)
    {
        _fileService = fileService;
        _userManager = userManager;
    }

    public async Task<Result<StartUploadFileResponse, ErrorList>> Handle(
        StartUploadPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validateResult = Photo.Validate(
            command.FileName,
            command.ContentType,
            command.FileSize);

        if (validateResult.IsFailure)
            return validateResult.Error.ToErrorList();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
        {
            return Errors.General.NotFound(command.UserId, nameof(command.UserId)).ToErrorList();
        }

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