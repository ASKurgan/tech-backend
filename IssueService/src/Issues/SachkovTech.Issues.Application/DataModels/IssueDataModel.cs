namespace SachkovTech.Issues.Application.DataModels;

public class IssueDataModel
{
    public Guid Id { get; init; }
    
    public bool IsDeleted { get; init; }
    
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
    
    public Guid? LessonId { get; init; }
    
    public Guid? ModuleId { get; init; }
    
    public Guid[] Files { get; set; } = [];
}