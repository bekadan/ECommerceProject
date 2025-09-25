using Catalog.Domain.Entities;

namespace Catalog.Application.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Adds a new category.
    /// </summary>
    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all categories including their products.
    /// </summary>
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single category by ID including its products.
    /// </summary>
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category.
    /// </summary>
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);

}
