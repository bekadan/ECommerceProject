using Core.Validation.Abstractions;
using Shared.Core;
using Shared.Core.Primitives;

namespace Catalog.Application.Commands.Products.Create;

public class CreateProductCommandValidator : IValidator<CreateProductCommand>
{
    public Task<Result> ValidateAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Task.FromResult(Result.Failure(Error.Create("Product name is required.")));

        if (command.Price < 0)
            return Task.FromResult(Result.Failure(Error.Create("Price cannot be negative.")));

        if (command.Stock < 0)
            return Task.FromResult(Result.Failure(Error.Create("Stock cannot be negative.")));

        if (string.IsNullOrWhiteSpace(command.Currency))
            return Task.FromResult(Result.Failure(Error.Create("Currency is required.")));

        return Task.FromResult(Result.Success());
    }
}
