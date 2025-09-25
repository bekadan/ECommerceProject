using Catalog.Application.DTOs;
using MediatR;
using Shared.Core;

namespace Catalog.Application.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
