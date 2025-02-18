using FileService.Options;
using Microsoft.Extensions.Options;

namespace FileService.Services;

/// <summary>
/// Определяет тип файла на основе его расширения
/// </summary>
public class FileTypeResolver
{
    private readonly FilesOptions _filesOptions;

    public FileTypeResolver(IOptions<FilesOptions> options)
    {
        _filesOptions = options.Value;
    }

    /// <summary>
    /// Определяет префикс бакета по расширению файла
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Тип файла.</returns>
    public string GetBucketNameByFileExtension(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        foreach (var type in _filesOptions.FileTypes.Where(type => type.Value.Contains(extension)))
        {
            return type.Key.ToLower();
        }

        return "other";
    }
}