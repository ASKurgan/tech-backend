using System.Net.Http.Json;
using AccountService.Contracts.Responses;
using CSharpFunctionalExtensions;

namespace AccountService.Communication;

internal class AccountHttpClient(HttpClient httpClient) : IAccountService
{
    public async Task<Result<ConfirmationLinkResponse, string>> GetConfirmationLink(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "confirmation-link/" + userId);

        var response = await httpClient.SendAsync(request, cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<ResponseWrapper<ConfirmationLinkResponse>>(cancellationToken)
                      ?? throw new Exception("ConfirmationLinkResponse can't be null");


        if (!response.IsSuccessStatusCode)
        {
            return payload.Errors.ToString()!;
        }

        if (payload.Result is null)
        {
            return "Confirmation link is null";
        }

        return payload.Result;
    }

    public class ResponseWrapper<T>
    {
        public required T? Result { get; set; }

        public required object Errors { get; set; }

        public DateTime TimeGenerated { get; set; }
    }
}