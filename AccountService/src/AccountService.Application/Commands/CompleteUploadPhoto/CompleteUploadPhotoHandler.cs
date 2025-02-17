using AccountService.Domain;
using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SharedKernel;

namespace AccountService.Application.Commands.CompleteUploadPhoto;

public class CompleteUploadPhotoHandler : ICommandHandler<CompleteUploadPhotoCommand>
{
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;

    public CompleteUploadPhotoHandler(
        IFileService fileService,
        UserManager<User> userManager)
    {
        _fileService = fileService;
        _userManager = userManager;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        CompleteUploadPhotoCommand command,
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

        var completeRequest = new CompleteMultipartRequest(command.UploadId, command.Parts);

        var result = await _fileService.CompleteMultipartUpload(completeRequest, cancellationToken);

        if (result.IsFailure)
        {
            return Errors.General.ValueIsInvalid(result.Error).ToErrorList();
        }

        var photo = new Photo(result.Value.FileId);

        user.Photo = photo;

        return UnitResult.Success<ErrorList>();
    }
}