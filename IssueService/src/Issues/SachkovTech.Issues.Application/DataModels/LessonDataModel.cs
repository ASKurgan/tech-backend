namespace SachkovTech.Issues.Application.DataModels;

public record LessonDataModel
{
    public Guid Id { get; init; }
    
    public Guid ModuleId { get; init; }
    
    public string Title { get; init; } = string.Empty;
    
    public string Description { get; init; } = string.Empty;
    
    public int Experience { get; init; }
    
    public Guid VideoId { get; init; }
    
    public Guid PreviewId { get; init; }

    public Guid[] Tags { get; init; } = [];
    
    public Guid[] Issues { get; init; }= [];
}