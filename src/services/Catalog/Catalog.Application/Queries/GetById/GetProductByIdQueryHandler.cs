using Caching.Core.Abstractions;
using Catalog.Application.DTOs;
using Catalog.Application.Repositories;
using Core.Exceptions.Types;
using Core.Logging.Abstractions;
using MediatR;
using Shared.Core;
using Shared.Core.Primitives;

namespace Catalog.Application.Queries.GetById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductAggregateRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public GetProductByIdQueryHandler(
        IProductAggregateRepository repository,
        ICacheService cacheService,
        ILogger logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{query.Id}";
        _logger.Information("Fetching product with ID {ProductId}", query.Id);

        try
        {
            // 1️⃣ Try cache
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
            if (cachedProduct is not null)
            {
                _logger.Information("Product {ProductId} retrieved from cache", query.Id);
                return Result.Success(cachedProduct);
            }

            // 2️⃣ Load from DB
            var aggregate = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (aggregate is null)
            {
                _logger.Warning("Product {ProductId} not found", query.Id);
                return Result.Failure<ProductDto>(Error.Create("PRODUCT_NOT_FOUND", $"Product with ID {query.Id} not found."));
            }

            // 3️⃣ Map to DTO
            var dto = new ProductDto
            {
                Id = aggregate.Product.Id,
                Name = aggregate.Product.Name.Value,
                Price = aggregate.Product.Price.Amount,
                Currency = aggregate.Product.Price.Currency,
                Stock = aggregate.Product.Stock,
                CategoryId = aggregate.Product.CategoryId,
                CategoryName = aggregate.Product.Category?.Name
            };

            // 4️⃣ Update cache
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromHours(1), cancellationToken);
            _logger.Information("Product {ProductId} loaded from DB and cached", query.Id);

            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            _logger.Error(ex, "Domain error while fetching product {ProductId}", query.Id);
            return Result.Failure<ProductDto>(Error.Create("DOMAIN_ERROR", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while fetching product {ProductId}", query.Id);
            return Result.Failure<ProductDto>(Error.Create("UNEXPECTED_ERROR", "An unexpected error occurred."));
        }
    }
}
