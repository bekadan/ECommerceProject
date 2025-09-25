using Catalog.Application.DTOs;
using MediatR;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Queries.GetAll;

public sealed record GetProductsQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<Result<PaginatedList<ProductDto>>>;
