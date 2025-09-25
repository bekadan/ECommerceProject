using Core.Logging.Abstractions;
using FluentValidation;

namespace Core.Validation.Base;

public abstract class BaseValidator<T> : AbstractValidator<T>, IValidator<T>
{
    private readonly ILogger _logger;

    protected BaseValidator(ILogger logger)
    {
        _logger = logger;
    }

    public async Task ValidateAndThrowAsync(T instance, CancellationToken cancellationToken = default)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);

        if (!result.IsValid)
        {
            IDictionary<string, string[]> errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var ex = new Exceptions.Types.ValidationException("Validation failed", errors);

            _logger.Error(ex, "Validation failed for {Type}. Errors: {@Errors}", typeof(T).Name, errors);

            throw ex;
        }
    }
}
