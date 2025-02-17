using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace AccountService.Application.Extensions;

public static class EmailExtensions
{
    public static string NormalizeBase64UrlStringAndGetResult(string input)
    {
        try
        {
            // Извлекаем токен из ссылки, предполагая, что он идет после последнего "/"
            string tokenPart = input.Substring(input.LastIndexOf('/') + 1);

            // Декодируем токен из Base64Url
            byte[] decodedBytes = WebEncoders.Base64UrlDecode(tokenPart);

            // Преобразуем байты в строку
            return Encoding.UTF8.GetString(decodedBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to process the input string. Ensure it is a valid confirmation link with a token.", ex);
        }
    }
}