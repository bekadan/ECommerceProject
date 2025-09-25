namespace Catalog.Application.DTOs.Products;

public class ProductDto
{
    public ProductDto(Guid ıd, string name, decimal price, string currency, int stock, Guid categoryId)
    {
        Id = ıd;
        Name = name;
        Price = price;
        Currency = currency;
        Stock = stock;
        CategoryId = categoryId;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
}
