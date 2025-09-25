using Caching.Core.Abstractions;
using Caching.Core.Options;
using Catalog.Application.DTOs.Categories;
using Catalog.Application.DTOs.Products;
using Catalog.Application.Repositories;
using Core.Logging.Abstractions;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;
using Shared.Core.Primitives;

namespace Catalog.Application.Queries.Categories.GetAll;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, Result<List<GetCategoriesWithProductsDto>>>
{
    private readonly ICategoryRepository _repository;
    private readonly IProductAggregateRepository _productAggregateRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public GetCategoriesQueryHandler(ICategoryRepository repository, ICacheService cacheService, ILogger logger, IProductAggregateRepository productAggregateRepository)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
        _productAggregateRepository = productAggregateRepository;
    }

    public async Task<Result<List<GetCategoriesWithProductsDto>>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        _logger.Information("Fetching categories with products");

        try
        {
            // Try cache first
            var categories = await _cacheService.GetOrAddAsync(
                CacheKeys.Categories,
                async () =>
                {
                    // 1️⃣ Fetch categories
                    var categoryEntities = await _repository.GetAllAsync(cancellationToken);

                    if (!categoryEntities.Any())
                        return new List<GetCategoriesWithProductsDto>();

                    // 2️⃣ Fetch products for all categories
                    var categoryIds = categoryEntities.Select(c => c.Id).ToList();
                    var productEntities = await _productAggregateRepository.GetByCategoryIdsAsync(categoryIds, cancellationToken);

                    // 3️⃣ Map to DTO
                    return categoryEntities.Select(c =>
                    {
                        var productsForCategory = productEntities
                            .Where(p => p.Product.CategoryId == c.Id)
                            .Select(p => new ProductDto(
                                p.Product.Id,
                                p.Product.Name.Value,
                                p.Product.Price.Amount,
                                p.Product.Price.Currency,
                                p.Product.Stock,
                                p.Product.CategoryId
                            ))
                            .ToList();

                        return new GetCategoriesWithProductsDto(c.Id, c.Name, productsForCategory);
                    }).ToList();
                },
                TimeSpan.FromMinutes(10),
                cancellationToken
            );

            if (categories == null || !categories.Any())
            {
                var error = Error.Create("No categories found");
                _logger.Warning("No categories found");
                return Result.Failure<List<GetCategoriesWithProductsDto>>(error);
            }

            _logger.Information("Fetched {Count} categories", categories.Count);
            return Result.Success(categories);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to fetch categories with products");
            return Result.Failure<List<GetCategoriesWithProductsDto>>(Error.Create("Failed to fetch categories"));
        }

    }
}
