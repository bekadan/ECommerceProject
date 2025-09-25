namespace Core.Validation.Abstractions;

public interface IValidator<T>
{
    Task ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
