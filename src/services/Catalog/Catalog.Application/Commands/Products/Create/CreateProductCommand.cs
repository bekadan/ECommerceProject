using Catalog.Application.DTOs.Products;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Commands.Products.Create;

public record CreateProductCommand(
    string Name,
    decimal Price,
    string Currency,
    Guid CategoryId,
    int Stock,
    string? CategoryName = null
) : ICommand<Result<ProductDto>>;

