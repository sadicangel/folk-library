using FluentValidation;
using FluentValidation.Results;

namespace FolkLibrary.Services;
internal sealed class ValidatorService<T> : IValidatorService<T>
{
    private readonly IEnumerable<IValidator<T>> _validators;

    public ValidatorService(IEnumerable<IValidator<T>> validators)
    {
        _validators = validators;
    }

    public async Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        var context = new ValidationContext<T>(value);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        return new ValidationResult(results.Where(r => r.IsValid).SelectMany(r => r.Errors));
    }
}
