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

public class CreateLessonHandler : ICommandHandler<Guid, CreateLessonCommand>
{
    private readonly IValidator<CreateLessonCommand> _validator;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IModulesRepository _modulesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateLessonHandler> _logger;
    private readonly IFileService _fileService;

    public CreateLessonHandler(
        IValidator<CreateLessonCommand> validator,
        ILessonsRepository lessonsRepository,
        IModulesRepository modulesRepository,
        IUnitOfWork unitOfWork,
        IFileService fileService,
        ILogger<CreateLessonHandler> logger)
    {
        _validator = validator;
        _lessonsRepository = lessonsRepository;
        _modulesRepository = modulesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        CreateLessonCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        (_, bool isFailure, Module? module, Error? error) =
            await _modulesRepository.GetById(command.ModuleId, cancellationToken);
        if (isFailure)
            return error.ToErrorList();

        var title = Title.Create(command.Title).Value;
        var isLessonExists = await _lessonsRepository.GetByTitle(title, cancellationToken);
        if (isLessonExists.IsSuccess)
            return Errors.General.AlreadyExist().ToErrorList();

        var lesson = new Lesson(
            LessonId.NewLessonId(),
            command.ModuleId,
            Title.Create(command.Title).Value,
            Description.Create(command.Description).Value,
            Experience.Create(command.Experience).Value,
            command.Tags.ToArray(),
            command.Issues.ToArray());

        await _lessonsRepository.Add(lesson, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        // TODO: реализовать через доменное событие
        module.AddLesson(lesson.Id);

        await _unitOfWork.SaveChanges(cancellationToken);

        var result = await _fileService.CompleteMultipartUpload(command.MultipartRequest, cancellationToken);
        if (result.IsFailure)
            return Errors.General.ValueIsInvalid(result.Error).ToErrorList();

        var video = new Video(Guid.Parse(result.Value.FileId));

        lesson.AddVideo(video);

        await _unitOfWork.SaveChanges(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.Log(LogLevel.Information, "Added new lesson with {LessonId}", lesson.Id);

        return lesson.Id.Value;
    }
}