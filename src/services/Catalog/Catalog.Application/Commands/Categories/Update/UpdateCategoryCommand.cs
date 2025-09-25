using Catalog.Application.DTOs.Categories;
using MediatR;
using Shared.Core;

namespace Catalog.Application.Commands.Categories.Update;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<Result<CreateCategoryDto>>;
