using CSharpFunctionalExtensions;
using SharedKernel;

namespace UserProgressService.Domain.ValueObjects;

public class Difficulty : ComparableValueObject
{
    private static List<string> _values => [None, Easy, Medium, Hard];

    public const string None = nameof(None);
    public const string Easy = nameof(Easy);
    public const string Medium = nameof(Medium);
    public const string Hard = nameof(Hard);

    public Difficulty(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Difficulty, Error> Create(string value)
    {
        if (_values.Any(v => v.ToLower() == value.ToLower()) == false)
            return Errors.General.ValueIsInvalid("Difficulty");
        
        return new Difficulty(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}