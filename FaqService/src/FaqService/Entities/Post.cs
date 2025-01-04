using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using FaqService.Enums;
using SharedKernel;
using static FaqService.Constants.Constants;

namespace FaqService.Entities;

public class Post : Entity<Guid>
{
    private Post(
        Guid id,
        string title,
        string description,
        string replLink,
        Guid userId,
        Guid? issueId,
        Guid? lessonId,
        List<Guid>? tags) : base(id)
    {
        Title = title;
        Description = description;
        ReplLink = replLink;
        UserId = userId;
        IssueId = issueId;
        LessonId = lessonId;
        Tags = tags;
        Status = Status.Opened;
        CreatedAt = DateTime.UtcNow;
    }

    public string Title { get; private set; } 
    public string Description { get; private set; } 
    public string ReplLink { get; private set; } 
    public Guid UserId { get; private set; } = default!;
    public Guid? IssueId { get; private set; } = default!;
    public Guid? LessonId { get; private set; } = default!;
    public List<Guid> Tags { get; private set; }
    public Status Status { get; private set; } = default!;
    public Guid? AnswerId { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = default!;

    private List<Answer> _answers = [];

    public IReadOnlyList<Answer> Answers => _answers;

    public static Result<Post, Error> Create(
        Guid id,
        string title,
        string description,
        string replLink,
        Guid userId,
        Guid? issueId,
        Guid? lessonId,
        List<Guid>? tags)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > LOW_TEXT_LENGTH)
            return Error.Validation("title.length", "Invalid title length");
        if (string.IsNullOrWhiteSpace(description) || description.Length > MAX_TEXT_LENGTH)
            return Error.Validation("description.length", "Invalid description length");
        if (string.IsNullOrWhiteSpace(replLink) || !Regex.IsMatch(replLink, patternRepLink))
            return Error.Validation("repLink.length", "Invalid repLink length");
        return new Post(
            id,
            title,
            description,
            replLink,
            userId,
            issueId,
            lessonId,
            tags);
    }

    public UnitResult<Error> AddAnswer(Answer answer)
    {
        _answers.Add(answer);
        return Result.Success<Error>();
    }

    public UnitResult<Error> UpdateMainInfo(
        string title,
        string description)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > LOW_TEXT_LENGTH)
            return Error.Validation("title.length", "Invalid title length");
        if (string.IsNullOrWhiteSpace(description) || description.Length > MAX_TEXT_LENGTH)
            return Error.Validation("description.length", "Invalid description length");
        Title = title;
        Description = description;
        return Result.Success<Error>();
    }

    public void UpdateRefsAndTags(
        string replLink,
        Guid? issueId,
        Guid? lessonId,
        List<Guid>? tags)
    {
        ReplLink = replLink;
        IssueId = issueId;
        LessonId = lessonId;
        Tags = tags;
    }

    public Result<Guid, Error> SelectSolution(Guid answerId)
    {
        var answerRes = _answers.FirstOrDefault(a => a.Id == answerId);
        if (answerRes == null)
            return Error.NotFound("answer", "Answer not found");
        answerRes.ChangeIsSolution(true);
        AnswerId = answerId;
        Status = Status.Closed;
        return answerId;
    }

    public UnitResult<Error> DeleteAnswer(Guid answerId)
    {
        var answer = Answers.FirstOrDefault(a => a.Id == answerId);
        if (answer == null)
            return Result.Success<Error>();
        
        _answers.Remove(answer);
        
        return Result.Success<Error>();
    }
}