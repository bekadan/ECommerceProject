using Catalog.Application.Repositories;
using Catalog.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductAggregateRepository : IProductAggregateRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductAggregateRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductAggregate?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .Include(p => p.Category) // optional, include category if needed
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return null; 

        var aggregate = new ProductAggregate(product, product.Category);
        return aggregate;
    }

    public async Task AddAsync(ProductAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        // Add category if exists and not tracked
        if (aggregate.Category is not null && !_dbContext.Categories.Local.Any(c => c.Id == aggregate.Category.Id))
        {
            _dbContext.Categories.Add(aggregate.Category);
        }

        _dbContext.Products.Add(aggregate.Product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProductAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        _dbContext.Products.Update(aggregate.Product);

        if (aggregate.Category is not null)
        {
            _dbContext.Categories.Update(aggregate.Category);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ProductAggregate> Products, int TotalCount)> GetPagedAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        // 1️⃣ Count total products for pagination
        var totalCount = await _dbContext.Products.CountAsync(cancellationToken);

        // 2️⃣ Query products
        var entities = await _dbContext.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // 3️⃣ Map EF entities → aggregate roots
        var aggregates = entities
            .Select(ProductAggregate.FromEntity)
            .ToList()
            .AsReadOnly();

        return (aggregates, totalCount);
    }
}
