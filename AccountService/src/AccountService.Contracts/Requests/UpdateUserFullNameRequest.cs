using AccountService.Contracts.Dtos;

namespace AccountService.Contracts.Requests;

public record UpdateUserFullNameRequest(FullNameDto FullName);