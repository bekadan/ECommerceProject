namespace Catalog.Application.DTOs.Categories;

public class CreateCategoryDto
{
    public CreateCategoryDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
}
