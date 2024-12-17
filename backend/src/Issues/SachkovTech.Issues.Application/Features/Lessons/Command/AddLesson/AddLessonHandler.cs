using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;

public class AddLessonHandler(
    IValidator<AddLessonCommand> validator,
    ILessonsRepository lessonsRepository,
    IModulesRepository modulesRepository,
    IFileService fileService,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<AddLessonHandler> logger) : ICommandHandler<Guid, AddLessonCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        AddLessonCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var moduleResult = await modulesRepository.GetById(command.ModuleId, cancellationToken);
        if(moduleResult.IsFailure)
            return moduleResult.Error.ToErrorList();
        var module = moduleResult.Value;
        
        var title = Title.Create(command.Title).Value;
        var isLessonExists = await lessonsRepository.GetByTitle(title, cancellationToken);
        if (isLessonExists.IsSuccess)
            return Errors.General.AlreadyExist().ToErrorList();

        var videoResult = await CompleteUploadVideo(
            command.FileName,
            command.ContentType,
            command.FileSize,
            command.UploadId,
            command.Parts,
            cancellationToken);

        if (videoResult.IsFailure)
            return videoResult.Error.ToErrorList();

        var lesson = CreateLesson(command, videoResult.Value);
        
        await lessonsRepository.Add(lesson, cancellationToken);
        module.AddLesson(lesson.Id);
        
        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Added new lesson with {LessonId}", lesson.Id);

        return lesson.Id.Value;
    }

    private Lesson CreateLesson(AddLessonCommand command, Video video) =>
        new(LessonId.NewLessonId(),
            command.ModuleId,
            Title.Create(command.Title).Value,
            Description.Create(command.Description).Value,
            Experience.Create(command.Experience).Value,
            video,
            command.PreviewId,
            command.Tags.ToArray(),
            command.Issues.ToArray());

    private async Task<Result<Video, Error>> CompleteUploadVideo(
        string fileName,
        string contentType,
        long fileSize,
        string uploadId,
        List<PartETagInfo> parts,
        CancellationToken cancellationToken)
    {
        var validateResult = Video.Validate(
            fileName,
            contentType,
            fileSize);

        if (validateResult.IsFailure)
            return validateResult.Error;

        var completeRequest = new CompleteMultipartRequest(uploadId, parts);

        var result = await fileService.CompleteMultipartUpload(completeRequest, cancellationToken);

        if (result.IsFailure)
            return Errors.General.ValueIsInvalid(result.Error);

        return new Video(result.Value.FileId);
    }
}