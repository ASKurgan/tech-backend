namespace FileService.Options;

public class FilesOptions
{
    public const string FILES = "FilesOptions";

    public Dictionary<string, List<string>> FileTypes { get; set; } = [];
}