namespace SachkovTech.Issues.Application.DataModels;

public record CommentDataModel(
    Guid Id,
    Guid UserId,
    string Message, 
    DateTime CreatedAt,
    Guid IssueReviewId);