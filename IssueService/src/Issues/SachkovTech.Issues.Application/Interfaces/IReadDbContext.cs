using SachkovTech.Issues.Application.DataModels;

namespace SachkovTech.Issues.Application.Interfaces;

public interface IReadDbContext
{
    IQueryable<IssueDataModel> Issues { get; }
    
    IQueryable<ModuleDataModel> Modules { get; }
    
    IQueryable<IssueReviewDataModel> IssueReviewDtos { get; }
    
    IQueryable<CommentDataModel> Comments { get; }
    
    IQueryable<UserIssueDataModel> UserIssues { get; }
    
    IQueryable<LessonDataModel> Lessons { get; }
}