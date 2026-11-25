using FluentValidation.Results;

namespace Time2Pay.BLL.Utils;

public sealed record ValidationViolation : Error
{
    private ValidationViolation(ValidationViolationItem[] items)
        : base("Validation.General", "One or more validation violations occurred", ErrorType.Validation)
    {
        Items = items;
    }

    public ValidationViolationItem[] Items { get; }

    public static ValidationViolation FromResult(ValidationResult validationResult)
    {
        var items = validationResult.Errors
            .Where(failure => failure is not null)
            .Select(failure => new ValidationViolationItem(failure.ErrorCode, failure.PropertyName, failure.ErrorMessage))
            .ToArray();

        return new ValidationViolation(items);
    }
}
