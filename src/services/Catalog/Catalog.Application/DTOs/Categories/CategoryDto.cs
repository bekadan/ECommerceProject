using Catalog.Application.DTOs.Products;

namespace Catalog.Application.DTOs.Categories;

public class GetCategoriesWithProductsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ProductDto> Products { get; set; }

    public GetCategoriesWithProductsDto(Guid id, string name, List<ProductDto> products)
    {
        Id = id;
        Name = name;
        Products = products;
    }
}