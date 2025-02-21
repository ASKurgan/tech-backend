namespace FaqService.Contracts;

public record CreateAnswerRequest(string Text, Guid UserId);