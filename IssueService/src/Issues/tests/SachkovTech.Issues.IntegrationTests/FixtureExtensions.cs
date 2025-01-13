using AutoFixture;
using SachkovTech.Issues.Application.Features.Issue.Commands.AddIssue;
using SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue;
using SachkovTech.Issues.Application.Features.Issue.Commands.RestoreIssue;
using SachkovTech.Issues.Application.Features.Issue.Commands.UpdateIssueMainInfo;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;
using SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonById;
using SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;
using SachkovTech.Issues.Application.Features.Modules.Commands.Create;
using SachkovTech.Issues.Application.Features.Modules.Commands.Delete;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateIssuePosition;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateLessonPosition;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateMainInfo;
using SachkovTech.Issues.Application.Features.Modules.Queries.GetModulesWithPagination;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.IntegrationTests;

public static class FixtureExtensions
{
    public static AddLessonCommand CreateAddLessonCommand(
        this IFixture fixture,
        Guid moduleId)
    {
        return fixture.Build<AddLessonCommand>()
            .With(c => c.ModuleId, moduleId)
            .With(c => c.FileName, "file.mp4")
            .With(c => c.ContentType, "video/mp4")
            .With(c => c.FileSize, 1024)
            .Create();
    }

    public static CreateIssueCommand CreateAddIssueCommand(
        this IFixture fixture,
        Guid moduleId,
        Guid lessonId)
    {
        return fixture.Build<CreateIssueCommand>()
            .With(c => c.LessonId, lessonId)
            .With(c => c.ModuleId, moduleId)
            .With(c => c.Experience, 5)
            .Create();
    }

    public static UpdateIssueMainInfoCommand CreateUpdateIssueMainInfoCommand(
        this IFixture fixture,
        Guid issueId,
        Guid lessonId,
        Guid moduleId)
    {
        return fixture.Build<UpdateIssueMainInfoCommand>()
            .With(c => c.IssueId, issueId)
            .With(c => c.LessonId, lessonId)
            .With(c => c.ModuleId, moduleId)
            .With(c => c.Experience, 5)
            .Create();
    }

    public static DeleteIssueCommand CreateDeleteIssueCommand(
        this IFixture fixture,
        Guid issueId)
    {
        return fixture.Build<DeleteIssueCommand>()
            .With(c => c.IssueId, issueId)
            .Create();
    }

    public static RestoreIssueCommand CreateRestoreIssueCommand(
        this IFixture fixture,
        Guid issueId)
    {
        return fixture.Build<RestoreIssueCommand>()
            .With(c => c.IssueId, issueId)
            .Create();
    }

    public static GetLessonByIdQuery CreateGetLessonByIdQuery(
        this IFixture fixture,
        Guid lessonId)
    {
        return fixture.Build<GetLessonByIdQuery>()
            .With(c => c.LessonId, lessonId)
            .Create();
    }

    public static GetLessonsWithPaginationQuery CreateGetLessonsWithPaginationQuery(
        this IFixture fixture,
        int page,
        int pageSize)
    {
        return fixture.Build<GetLessonsWithPaginationQuery>()
            .With(c => c.Page, page)
            .With(c => c.PageSize, pageSize)
            .Create();
    }

    public static CreateModuleCommand CreateCreateModuleCommand(this IFixture fixture)
    {
        return fixture.Create<CreateModuleCommand>();
    }

    public static DeleteModuleCommand CreateDeleteModuleCommand(this IFixture fixture, Guid moduleId)
    {
        return fixture.Build<DeleteModuleCommand>()
            .With(c => c.ModuleId, moduleId)
            .Create();
    }

    public static UpdateMainInfoCommand CreateUpdateMainInfoCommand(this IFixture fixture, Guid moduleId)
    {
        return fixture.Build<UpdateMainInfoCommand>()
            .With(c => c.ModuleId, moduleId)
            .Create();
    }

    public static UpdateIssuePositionCommand CreateUpdateIssuePositionCommand(
        this IFixture fixture,
        Guid moduleId,
        Guid issueId,
        int position)
    {
        return fixture.Build<UpdateIssuePositionCommand>()
            .With(c => c.ModuleId, moduleId)
            .With(c => c.IssueId, issueId)
            .With(c => c.NewPosition, position)
            .Create();
    }

    public static UpdateLessonPositionCommand CreateUpdateLessonPositionCommand(
        this IFixture fixture,
        Guid moduleId,
        Guid lessonId,
        int position)
    {
        return fixture.Build<UpdateLessonPositionCommand>()
            .With(c => c.ModuleId, moduleId)
            .With(c => c.LessonId, lessonId)
            .With(c => c.Position, position)
            .Create();
    }

    public static GetModulesWithPaginationQuery CreateGetModulesQuery(this IFixture fixture)
    {
        return fixture.Build<GetModulesWithPaginationQuery>()
            .Create();
    }

    public static Module CreateModule(this IFixture fixture)
    {
        return new Module(
            ModuleId.NewModuleId(),
            Title.Create(fixture.Create<string>()).Value,
            Description.Create(fixture.Create<string>()).Value);
    }
}