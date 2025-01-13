using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;

public class AddLessonHandler : ICommandHandler<Guid, AddLessonCommand>
{
    private readonly IValidator<AddLessonCommand> _validator;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IModulesRepository _modulesRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddLessonHandler> _logger;

    public AddLessonHandler(
        IValidator<AddLessonCommand> validator,
        ILessonsRepository lessonsRepository,
        IModulesRepository modulesRepository,
        IFileService fileService,
        IUnitOfWork unitOfWork,
        ILogger<AddLessonHandler> logger)
    {
        _validator = validator;
        _lessonsRepository = lessonsRepository;
        _modulesRepository = modulesRepository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        AddLessonCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        (_, bool isFailure, Module? module, Error? error) =
            await _modulesRepository.GetById(command.ModuleId, cancellationToken);
        if (isFailure)
            return error.ToErrorList();

        var title = Title.Create(command.Title).Value;
        var isLessonExists = await _lessonsRepository.GetByTitle(title, cancellationToken);
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

        await _lessonsRepository.Add(lesson, cancellationToken);

        module.AddLesson(lesson.Id);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.Log(LogLevel.Information, "Added new lesson with {LessonId}", lesson.Id);

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

        var result = await _fileService.CompleteMultipartUpload(completeRequest, cancellationToken);

        if (result.IsFailure)
            return Errors.General.ValueIsInvalid(result.Error);

        return new Video(result.Value.FileId);
    }
}