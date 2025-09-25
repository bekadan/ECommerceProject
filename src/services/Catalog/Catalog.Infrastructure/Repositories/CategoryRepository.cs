using Catalog.Application.Repositories;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _dbContext;

    public CategoryRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new category.
    /// </summary>
    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category is null) throw new ArgumentNullException(nameof(category));

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Returns all categories including their products.
    /// </summary>
    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .OrderBy(c => c.Name)       // Order by name
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns a single category by ID including its products.
    /// </summary>
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category is null) throw new ArgumentNullException(nameof(category));

        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category is null) throw new ArgumentNullException(nameof(category));

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
