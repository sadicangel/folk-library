using FluentValidation.Results;

namespace FolkLibrary.Services;

public interface IValidatorService<T>
{
    public Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default);
}