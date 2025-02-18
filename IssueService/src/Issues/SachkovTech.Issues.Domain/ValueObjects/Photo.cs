using CSharpFunctionalExtensions;
using SharedKernel;

namespace SachkovTech.Issues.Domain.ValueObjects;

public class Photo : ComparableValueObject
{
    private static string[] PERMITED_FILES_TYPE = { "image/jpg", "image/jpeg", "image/png", "image/gif" };

    private static string[] PERMITED_EXTENSIONS = { "jpg", "jpeg", "png", "gif" };

    private static long MAX_FILE_SIZE = 5242880;

    public Photo(Guid fileId)
    {
        FileId = fileId;
    }

    public Guid FileId { get; }

    // public static Result<Photo, Error> Create(Guid fileId) => new Photo(fileId);
    public static UnitResult<Error> Validate(
        string fileName,
        string contentType,
        long size)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Errors.General.ValueIsInvalid(fileName);
        }

        var fileExtension = fileName[fileName.LastIndexOf('.')..];

        if (PERMITED_EXTENSIONS.All(x => x != fileExtension))
        {
            return Errors.General.Failure();
        }

        if (PERMITED_FILES_TYPE.All(x => x != contentType))
        {
            return Errors.General.ValueIsInvalid(contentType);
        }

        if (size > MAX_FILE_SIZE)
        {
            return Errors.General.Failure();
        }

        return Result.Success<Error>();
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return FileId;
    }
}