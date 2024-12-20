namespace AccountService.Contracts.Responses;

public record ConfirmationLinkResponse(string Email, string ConfirmationLink);