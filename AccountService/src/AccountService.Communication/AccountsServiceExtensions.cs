using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AccountService.Communication;

public static class AccountsServiceExtensions
{
    public static IHttpClientBuilder AddAccountHttpCommunication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AccountsServiceOptions>(configuration.GetSection(AccountsServiceOptions.ACCOUNTS_SERVICE));

        return services.AddHttpClient<IAccountService, AccountHttpClient>((sp, config) =>
        {
            var accountsOptions = sp.GetRequiredService<IOptions<AccountsServiceOptions>>().Value;

            config.BaseAddress = new Uri(accountsOptions.Url);
        });
    }
}