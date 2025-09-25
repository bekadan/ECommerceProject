using Caching.Core.Abstractions;
using Caching.Core.Options;
using Caching.Core.Extensions;
using Catalog.Application.Repositories;
using Core.Logging.Abstractions;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;
using Shared.Core.Primitives;
using Catalog.Application.DTOs.Products;

namespace Catalog.Application.Queries.Products.GetAll;

public class GetProductsQueryHandler
    : IQueryHandler<GetProductsQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IProductAggregateRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public GetProductsQueryHandler(IProductAggregateRepository repository, ICacheService cacheService, ILogger logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var page = query.PageNumber;
        var size = query.PageSize;

        _logger.Information("Fetching products page {Page} size {Size}", page, size);

        try
        {
            // 1️⃣ Get data from cache or repository
            var repoResult = await _cacheService.GetOrAddPaginatedAsync(
                CacheKeys.Products,
                page,
                size,
                async () => await _repository.GetPagedAsync(page, size, cancellationToken),
                TimeSpan.FromMinutes(10),
                cancellationToken
            );

            if (repoResult.Products == null || repoResult.Products.Count == 0)
            {
                var error = Error.Create("No products found");
                _logger.Warning("No products found on page {Page}", page);
                return Result.Failure<PaginatedList<ProductDto>>(error);
            }

            // 2️⃣ Map ProductAggregate → ProductDto
            var productDtos = repoResult.Products.Select(p => new ProductDto(
                    p.Product.Id,
                    p.Product.Name.Value,
                    p.Product.Price.Amount,
                    p.Product.Price.Currency,
                    p.Product.Stock,
                    p.Product.CategoryId
                )).ToList();

            // 3️⃣ Wrap in PaginatedList
            var paginatedList = new PaginatedList<ProductDto>(productDtos, repoResult.TotalCount, page, size);

            _logger.Information("Fetched {Count} products for page {Page}", productDtos.Count, page);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to fetch products page {Page}", page);
            return Result.Failure<PaginatedList<ProductDto>>(Error.Create("Failed to fetch products"));
        }
    }

    
}

