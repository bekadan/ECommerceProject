using Catalog.Application.DTOs;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Commands.Create;

public record CreateProductCommand(
    string Name,
    decimal Price,
    string Currency,
    Guid CategoryId,
    int Stock,
    string? CategoryName = null
) : ICommand<Result<ProductDto>>;

