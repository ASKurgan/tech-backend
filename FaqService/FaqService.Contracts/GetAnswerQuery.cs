namespace FaqService.Contracts;

public record GetAnswerQuery(Guid? Cursor, int Limit = 10);