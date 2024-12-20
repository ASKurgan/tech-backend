using Microsoft.AspNetCore.Authentication;

namespace SachkovTech.Framework.Authorization;

public class SecretKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; init; } = "X-Internal-Service-Key";

    public string ExpectedKey { get; set; } = string.Empty;
}