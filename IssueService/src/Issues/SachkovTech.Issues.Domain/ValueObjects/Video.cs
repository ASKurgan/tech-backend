using CSharpFunctionalExtensions;
using SharedKernel;

namespace SachkovTech.Issues.Domain.ValueObjects;

public class Video : ComparableValueObject
{
    public const string LOCATION = "videos";

    public static readonly Video None = new(Guid.Empty);

    private const long MAX_FILE_SIZE_BYTES = 5_368_709_120;

    private static readonly string[] _availableFilesType =
        ["video/mp4", "video/mkv", "video/avi", "video/mov"];

    private static readonly string[] _availableExtensions =
        ["mp4", "mkv", "avi", "mov"];

    public Video(Guid fileId)
    {
        FileId = fileId;
    }

    public Guid FileId { get; }

    public string FileLocation { get; } = LOCATION;

    public static UnitResult<Error> Validate(string fileName, string contentType, long size)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Errors.General.ValueIsInvalid(fileName);
        }

        int lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1 || lastDotIndex == fileName.Length - 1)
        {
            return Errors.General.Failure();
        }

        // Извлекаем расширение без точки
        string fileExtension = fileName[(lastDotIndex + 1)..];
        if (!_availableExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return Errors.General.Failure();
        }

        if (!_availableFilesType.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            return Errors.General.ValueIsInvalid(contentType);
        }

        if (size > MAX_FILE_SIZE_BYTES)
        {
            return Errors.General.Failure();
        }

        return Result.Success<Error>();
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return FileId;
        yield return FileLocation;
    }
}