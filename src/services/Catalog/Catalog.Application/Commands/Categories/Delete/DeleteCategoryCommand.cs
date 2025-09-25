using MediatR;
using Shared.Core;

namespace Catalog.Application.Commands.Categories.Delete;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result<bool>>;
