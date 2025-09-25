using Shared.Core;

namespace Core.Validation.Abstractions;

public interface IValidator<T>
{
    Task<Result> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
