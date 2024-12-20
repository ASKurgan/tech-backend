

using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.StartUploadVideo;
public record StartUploadVideoCommand(
    string FileName,
    string ContentType,
    long FileSize) : ICommand;
