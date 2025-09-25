using Catalog.Application.DTOs.Categories;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;

namespace Catalog.Application.Commands.Categories.Create;

public record CreateCategoryCommand(string Name) : ICommand<Result<CreateCategoryDto>>;
