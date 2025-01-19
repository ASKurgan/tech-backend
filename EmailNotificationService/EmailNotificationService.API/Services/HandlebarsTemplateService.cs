using HandlebarsDotNet;
using Microsoft.Extensions.Caching.Memory;

namespace EmailNotificationService.API.Services;

/// <summary>
/// Templating service.
/// </summary>
public class HandlebarsTemplateService
{
    private readonly IMemoryCache _cache;

    public HandlebarsTemplateService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Seeks for compiled handlebar template in cache, if there is no such one, compiles it and saves in cache.
    /// Then substitutes reciepient-specific data into template.
    /// </summary>
    /// <param name="details">Dictionary with all recipient-specific data, 
    /// which would be inserted in email template.</param>
    /// <param name="templateKey">Email template name (key for searching template).</param>
    /// <returns>Email body as string.</returns>
    /// <exception cref="ApplicationException"></exception>
    public string Process(Dictionary<string, string> details, string templateKey)
    {
        if (!_cache.TryGetValue(templateKey, out HandlebarsTemplate<object, object>? compiledTemplate))
        {
            compiledTemplate = Handlebars.Compile(
                File.ReadAllText(Path.Combine(
                    Directory.GetCurrentDirectory(), 
                    "wwwroot", 
                    "Templates", 
                    $"{templateKey}.html")));

            _cache.Set(templateKey, compiledTemplate);
        }

        if (compiledTemplate is null)
            throw new ApplicationException("Template error");

        return compiledTemplate(details);
    }
}