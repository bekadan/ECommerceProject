using Catalog.Application.DTOs.Products;
using MediatR;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Queries.Products.GetAll;

public sealed record GetProductsQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<Result<PaginatedList<ProductDto>>>;
