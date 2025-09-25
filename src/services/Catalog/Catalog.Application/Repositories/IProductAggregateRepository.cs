using Catalog.Domain.Aggregates;

namespace Catalog.Application.Repositories;

public interface IProductAggregateRepository
{
    Task<ProductAggregate?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(ProductAggregate aggregate, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProductAggregate aggregate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductAggregate>> GetByCategoryIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProductAggregate> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
