using CSharpFunctionalExtensions;
using SharedKernel;

namespace ScheduleService.Domain.ValueObjects;

public class Description : ComparableValueObject
{
    public const int MAX_LENGTH = 2000;

    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, Error> Create(string value)
    {
        // В библе нет Errors
        // if (string.IsNullOrWhiteSpace(value) || value.Length > MAX_LENGTH)
        //     return Errors.General.ValueIsInvalid("Description");

        return new Description(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}