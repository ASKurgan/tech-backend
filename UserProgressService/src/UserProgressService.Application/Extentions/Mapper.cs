using UserProgressService.Application.Dtos;
using UserProgressService.Domain.ValueObjects.Conditions;

namespace UserProgressService.Application.Extentions;

public class Mapper
{
    public static ConditionDto MapConditionToDto(Condition condition)
    {
        return condition switch
        {
            IssueCondition issueCondition => new IssueConditionDto
            {
                TimeToComplete = issueCondition.TimeToComplete,
                Difficulty = issueCondition.Difficulty.Value,
                Attempts = issueCondition.Attempts,
                IssueCount = issueCondition.IssueCount,
            },
            LessonCondition lessonCondition => new LessonConditionDto
            {
                TimeToComplete = lessonCondition.TimeToComplete,
                Difficulty = lessonCondition.Difficulty.Value,
                RequiredCount = lessonCondition.RequiredCount,
            },
            _ => throw new InvalidOperationException("Unknown condition type")
        };
    }
}