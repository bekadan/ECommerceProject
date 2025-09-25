using Catalog.Application.DTOs.Categories;
using MediatR;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Queries.Categories.GetAll;

public record GetCategoriesQuery() : IQuery<Result<List<GetCategoriesWithProductsDto>>>;
