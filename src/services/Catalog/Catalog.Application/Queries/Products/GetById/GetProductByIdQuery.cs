using Catalog.Application.DTOs.Products;
using MediatR;
using Shared.Core;

namespace Catalog.Application.Queries.Products.GetById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
