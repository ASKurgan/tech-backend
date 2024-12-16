using CSharpFunctionalExtensions;
using SharedKernel;

namespace SachkovTech.SharedKernel.ValueObjects;

public class Video : ComparableValueObject
{
    public static readonly string[] PermitedFilesType =
        ["video/mp4", "video/mkv", "video/avi", "video/mov"];

    public static readonly string[] PermitedExtensions =
        ["mp4", "mkv", "avi", "mov"];

    public const long MAX_FILE_SIZE = 4294967296;

    public Video(Guid fileId)
    {
        FileId = fileId;
    }

    public Guid FileId { get; }

    public static UnitResult<Error> Validate(string fileName, string contentType, long size)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Errors.General.ValueIsInvalid(fileName);
        }

        var lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1 || lastDotIndex == fileName.Length - 1)
        {
            return Errors.Files.InvalidExtension();
        }

        // Извлекаем расширение без точки
        var fileExtension = fileName[(lastDotIndex + 1)..];
        if (!PermitedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return Errors.Files.InvalidExtension();
        }

        if (!PermitedFilesType.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            return Errors.General.ValueIsInvalid(contentType);
        }

        if (size > MAX_FILE_SIZE)
        {
            return Errors.Files.InvalidSize();
        }

        return Result.Success<Error>();
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return FileId;
    }
}