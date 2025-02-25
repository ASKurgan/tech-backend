using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SachkovTech.Framework.Swagger;

/// <summary>
/// Предоставляет методы расширения для настройки Swagger с использованием параметров из конфигурации.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Регистрирует Swagger документацию в коллекции сервисов, привязывая параметры API из конфигурации.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации Swagger.</param>
    /// <param name="configuration">Конфигурация приложения, содержащая секцию "Swagger".</param>
    /// <returns>Модифицированная коллекция сервисов с добавленным Swagger.</returns>
    public static IServiceCollection AddCustomSwagger(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Привязываем параметры описания API из конфигурационной секции "Swagger"
        var apiOptions = configuration.GetSection("Swagger").Get<ApiDescriptionOptions>()
                         ?? new ApiDescriptionOptions();

        services.AddSwaggerGen(options =>
        {
            ConfigureSwaggerDoc(options, apiOptions);
            ConfigureSecurity(options);
        });

        return services;
    }

    /// <summary>
    /// Настраивает описание Swagger документа с использованием переданных параметров API.
    /// </summary>
    /// <param name="options">Объект настроек SwaggerGen.</param>
    /// <param name="apiOptions">Параметры описания API, полученные из конфигурации.</param>
    private static void ConfigureSwaggerDoc(SwaggerGenOptions options, ApiDescriptionOptions apiOptions)
    {
        options.SwaggerDoc(
            apiOptions.Version,
            new OpenApiInfo
            {
                Title = apiOptions.Title,
                Version = apiOptions.Version,
                Description = apiOptions.Description,
                Contact = new OpenApiContact
                {
                    Name = apiOptions.Contact.Name,
                    Email = apiOptions.Contact.Email,
                    Url = new Uri(apiOptions.Contact.Url),
                },
            });
    }

    /// <summary>
    /// Настраивает схему безопасности для Swagger документации, используя JWT Bearer токены.
    /// </summary>
    /// <param name="options">Объект настроек SwaggerGen.</param>
    private static void ConfigureSecurity(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Description = "Введите JWT токен в формате: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                Array.Empty<string>()
            },
        });
    }
}