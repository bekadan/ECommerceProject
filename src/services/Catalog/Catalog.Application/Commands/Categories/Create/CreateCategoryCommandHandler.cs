using Caching.Core.Abstractions;
using Caching.Core.Options;
using Catalog.Application.DTOs.Categories;
using Catalog.Application.Repositories;
using Catalog.Domain.Entities;
using Core.Events.Dispatching;
using Core.Exceptions.Types;
using Core.Logging.Abstractions;
using Core.Validation.Abstractions;
using Shared.Core;
using Shared.Core.Abstractions.Messaging;
using Shared.Core.Primitives;

namespace Catalog.Application.Commands.Categories.Create;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Result<CreateCategoryDto>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger _logger;
    private readonly IValidator<CreateCategoryCommand> _validator;

    public CreateCategoryCommandHandler(
        ICategoryRepository repository,
        ICacheService cacheService,
        IEventDispatcher eventDispatcher,
        ILogger logger,
        IValidator<CreateCategoryCommand> validator)
    {
        _repository = repository;
        _cacheService = cacheService;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<CreateCategoryDto>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        _logger.Information("Creating category {CategoryName}", command.Name);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsFailure)
        {
            _logger.Warning("Validation failed for category {CategoryName}: {Error}", command.Name, validationResult.Error.Message);
            return Result.Failure<CreateCategoryDto>(validationResult.Error);
        }

        try
        {
            var category = new Category(command.Name);

            await _repository.AddAsync(category, cancellationToken);

            // Publish domain events if any
            foreach (var domainEvent in category.DomainEvents)
                await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);

            // Remove cached categories list
            await _cacheService.RemoveByPrefixAsync(CacheKeys.Categories, cancellationToken);

            var dto = new CreateCategoryDto(category.Id, category.Name);

            // Optionally cache single category
            var key = _cacheService.AddPrefix(CacheKeys.Categories, category.Id.ToString());
            await _cacheService.SetAsync(key, dto, TimeSpan.FromHours(1), cancellationToken);

            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            _logger.Error(ex, "Domain error while creating category {CategoryName}", command.Name);
            return Result.Failure<CreateCategoryDto>(Error.Create(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while creating category {CategoryName}", command.Name);
            return Result.Failure<CreateCategoryDto>(Error.Create("An unexpected error occurred."));
        }
    }
}
