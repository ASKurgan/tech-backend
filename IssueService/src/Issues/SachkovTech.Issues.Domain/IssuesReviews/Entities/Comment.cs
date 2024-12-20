using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.IssuesReviews.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Domain.IssuesReviews.Entities;

public class Comment : Entity<CommentId>
{
    //Ef core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Comment(CommentId id) : base(id) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Comment(CommentId id,
        UserId userId,
        Message message,
        DateTime createdAt) : base(id)
    {
        UserId = userId;
        Message = message;
        CreatedAt = createdAt;
    }

    public IssueReview? IssueReview { get; private set; }
    public UserId UserId { get; private set; }

    public Message Message { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Result<Comment, Error> Create(UserId userId,
        Message message)
    {
        return Result.Success<Comment, Error>(new Comment(
            CommentId.NewCommentId(),
            userId,
            message,
            DateTime.UtcNow));
    }
}