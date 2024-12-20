using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.StartUploadFile;
public record StartUploadPhotoCommand(
    Guid UserId,
    string FileName, 
    string ContentType, 
    long FileSize) : ICommand;
